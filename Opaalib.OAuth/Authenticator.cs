using Newtonsoft.Json;
using Opaalib.Shared;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Opaalib.OAuth
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

        // TODO: Make json converting awaited maybe
        // TODO: Remove url strings
        /// <exception cref="AuthenticationException">Thrown when the authentication fails</exception>
        public async Task<AccessTokenResponse> RequestAccessTokenAsync()
        {
            // This is a hack to force TLS 1.2. Does not work if you don't have .NET 4.5 installed
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

            using (var client = new MyWebClient())
            {
                client.Credentials = new NetworkCredential(Username, Password);
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                byte[] responseBytes = null;
                try
                {
                    // HACK: Using StartNew because .NET 4.0 doesn't have method which returns a Task
                    var task = TaskEx.Run(() => client.UploadValues($"{Config.BaseAddress}/token", "POST", new NameValueCollection
                    {
                        {  "grant_type", "client_credentials" }
                    }));
                    responseBytes = await task;
                }
                catch (WebException ex) when (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = (HttpWebResponse)ex.Response;
                    var statusCode = response.StatusCode;

                    string responseJson;
                    using (var stream = response.GetResponseStream())
                    using (var sr = new StreamReader(stream))
                    {
                        responseJson = sr.ReadToEnd();
                    }

                    if (statusCode == HttpStatusCode.Unauthorized) throw new AuthenticationException("Authentication failed", ex, responseJson);
                    if (statusCode == HttpStatusCode.BadRequest) throw new AuthenticationException("Invalid request", ex, responseJson);
                    if (statusCode == HttpStatusCode.Forbidden)
                    {
                        // TODO: Handle policy exceptions
                        throw new AuthenticationException("Request failed due to policy exception", ex, responseJson);
                    }

                    throw new AuthenticationException("Request failed due to unknown web exception", ex, responseJson);
                }
                catch (Exception ex)
                {
                    throw new AuthenticationException("Request failed due to unknown reason", ex);
                }

                try
                {
                    var response = Encoding.UTF8.GetString(responseBytes);
                    return JsonConvert.DeserializeObject<AccessTokenResponse>(response);
                }
                catch (Exception ex)
                {
                    throw new AuthenticationException("Response reading failed due to unknown reason", ex);
                }
            }
        }
    }
}
