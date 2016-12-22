using Newtonsoft.Json;
using SoneraOMA.Shared;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SoneraOMA.OAuth
{
    public class Authenticator
    {
        public string Username { get; }
        public string Password { get; }
        public AuthenticatorConfiguration Config { get; }

        public Authenticator(string username, string password, AuthenticatorConfiguration config)
        {
            Username = username;
            Password = password;
            Config = config;
        }

        // TODO: Error handling
        // TODO: Make json converting awaited maybe
        // TODO: Remove url strings
        public async Task<Result<AccessTokenResponse, AccessTokenError>> RequestAccessTokenAsync()
        {
            Result<AccessTokenResponse, AccessTokenError> CreateFailure(AccessTokenError error) => 
                Result.CreateFailure<AccessTokenResponse, AccessTokenError>(error);

            using (var client = new WebClient())
            {
                client.Credentials = new NetworkCredential(Username, Password);
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                byte[] responseBytes = null;
                try
                {
                    responseBytes = await client.UploadValuesTaskAsync($"{Config.CombinedAddress}/token", "POST", new NameValueCollection
                    {
                        {  "grant_type", "client_credentials" }
                    });
                }
                catch (WebException ex) when (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var statusCode = ((HttpWebResponse)ex.Response).StatusCode;
                    if (statusCode == HttpStatusCode.Unauthorized) return CreateFailure(AccessTokenError.AuthenticationFailed);
                    if (statusCode == HttpStatusCode.BadRequest) return CreateFailure(AccessTokenError.InvalidRequest);
                    if (statusCode == HttpStatusCode.Forbidden)
                    {
                        // TODO: Handle policy exceptions
                        return CreateFailure(AccessTokenError.RequestFailed);
                    }

                    return CreateFailure(AccessTokenError.RequestFailed);
                }
                catch (Exception)
                {
                    return CreateFailure(AccessTokenError.RequestFailed);
                }

                try
                {
                    var response = Encoding.UTF8.GetString(responseBytes);
                    return Result.CreateSuccess<AccessTokenResponse, AccessTokenError>(JsonConvert.DeserializeObject<AccessTokenResponse>(response));
                }
                catch (Exception)
                {
                    return CreateFailure(AccessTokenError.InvalidResponse);
                }
            }
        }
    }
}
