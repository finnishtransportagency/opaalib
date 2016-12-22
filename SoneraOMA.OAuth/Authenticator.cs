﻿using Newtonsoft.Json;
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

        // TODO: Make json converting awaited maybe
        // TODO: Remove url strings
        /// <exception cref="AuthenticationException">Thrown when the authentication fails</exception>
        public async Task<AccessTokenResponse> RequestAccessTokenAsync()
        {
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
                    if (statusCode == HttpStatusCode.Unauthorized) throw new AuthenticationException("Authentication failed", ex);
                    if (statusCode == HttpStatusCode.BadRequest) throw new AuthenticationException("Invalid request", ex);
                    if (statusCode == HttpStatusCode.Forbidden)
                    {
                        // TODO: Handle policy exceptions
                        throw new AuthenticationException("Request failed due to policy exception", ex);
                    }

                    throw new AuthenticationException("Request failed due to unknown web exception", ex);
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
