using Newtonsoft.Json;
using SoneraOMA.OAuth;
using SoneraOMA.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SoneraOMA.Messaging
{
    public class Messenger
    {
        public Authenticator Authenticator { get; }
        public MessengerConfiguration Config { get; }

        private DateTime accessTokenExpires;
        private AccessTokenResponse latestAccessToken;

        public Messenger(Authenticator authenticator, MessengerConfiguration messengerConfiguration)
        {
            Authenticator = authenticator;
            Config = messengerConfiguration;
        }

        /// <exception cref="AuthenticationException">Thrown when the authentication fails</exception>
        /// <exception cref="OutboundMessageException">Thrown when sending of outbound message request fails</exception>
        public async Task<OutboundMessageResponse> OutboundMessageRequestAsync(OutboundMessageRequestContainer outboundMessage)
        {
            await RefreshAccessTokenIfNeededAsync();

            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.Authorization] = $"Bearer {latestAccessToken.AccessToken}";
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.Headers[HttpRequestHeader.Accept] = "application/json";
                byte[] responseBytes = null;
                try
                {
                    var jsonStr = JsonConvert.SerializeObject(outboundMessage);
                    var jsonBytes = Encoding.UTF8.GetBytes(jsonStr);

                    responseBytes = await client.UploadDataTaskAsync(
                        $"{Config.CombinedAddress}/outbound/{Config.SenderAddress}/requests", "POST", jsonBytes);
                }
                catch (WebException ex) when (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var statusCode = ((HttpWebResponse)ex.Response).StatusCode;
                    if (statusCode == HttpStatusCode.Unauthorized) throw new OutboundMessageException("Authentication failed", ex);
                    if (statusCode == HttpStatusCode.BadRequest) throw new OutboundMessageException("Invalid request", ex);
                    if (statusCode == HttpStatusCode.Forbidden)
                    {
                        // TODO: Handle policy exceptions
                        throw new OutboundMessageException("Request failed due to policy exception", ex);
                    }

                    throw new OutboundMessageException("Request failed due to unknown web exception", ex);
                }
                catch (Exception ex)
                {
                    throw new OutboundMessageException("Request failed due to unknown reason", ex);
                }

                try
                {
                    var response = Encoding.UTF8.GetString(responseBytes);
                    return JsonConvert.DeserializeObject<OutboundMessageResponse>(response);
                }
                catch (Exception ex)
                {
                    throw new OutboundMessageException("Response reading failed due to unknown reason", ex);
                }
            }
        }

        /// <exception cref="AuthenticationException">Thrown when the authentication fails</exception>
        public async Task RefreshAccessTokenIfNeededAsync()
        {
            if (latestAccessToken != null && (DateTime.UtcNow - accessTokenExpires).TotalSeconds > 10) return;

            latestAccessToken = await Authenticator.RequestAccessTokenAsync();
            accessTokenExpires = DateTime.UtcNow + TimeSpan.FromSeconds(latestAccessToken.ExpiresIn);
        }
    }
}
