using Newtonsoft.Json;
using Opaalib.OAuth;
using Opaalib.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Opaalib.Messaging
{
    public class Messenger
    {
        public Authenticator Authenticator { get; }
        public MessengerConfiguration Config { get; }

        private DateTime accessTokenExpires;
        private AccessTokenResponse latestAccessToken;
        private object accessTokenLock = new object();

        public Messenger(Authenticator authenticator, MessengerConfiguration messengerConfiguration)
        {
            Authenticator = authenticator;
            Config = messengerConfiguration;
        }

        /// <exception cref="AuthenticationException">Thrown when the authentication fails</exception>
        /// <exception cref="MessengerException">Thrown when sending of outbound message request fails</exception>
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
                    if (statusCode == HttpStatusCode.Unauthorized) throw new MessengerException("Authentication failed", ex);
                    if (statusCode == HttpStatusCode.BadRequest) throw new MessengerException("Invalid request", ex);
                    if (statusCode == HttpStatusCode.Forbidden)
                    {
                        // TODO: Handle policy exceptions
                        throw new MessengerException("Request failed due to policy exception", ex);
                    }

                    throw new MessengerException("Request failed due to unknown web exception", ex);
                }
                catch (Exception ex)
                {
                    throw new MessengerException("Request failed due to unknown reason", ex);
                }

                try
                {
                    var response = Encoding.UTF8.GetString(responseBytes);
                    return JsonConvert.DeserializeObject<OutboundMessageResponse>(response);
                }
                catch (Exception ex)
                {
                    throw new MessengerException("Response reading failed due to unknown reason", ex);
                }
            }
        }

        /// <exception cref="AuthenticationException">Thrown when the authentication fails</exception>
        /// <exception cref="MessengerException">Thrown when reading delivery status fails</exception>
        public async Task<DeliveryInfoListContainer> ReadOutboundMessageDeliveryStatusAsync(string requestId)
        {
            await RefreshAccessTokenIfNeededAsync();

            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.Authorization] = $"Bearer {latestAccessToken.AccessToken}";
                client.Headers[HttpRequestHeader.Accept] = "application/json";
                byte[] responseBytes = null;
                try
                {
                    responseBytes = await client.DownloadDataTaskAsync(
                        $"{Config.CombinedAddress}/outbound/{Config.SenderAddress}/requests/{requestId}/deliveryInfos");

                }
                catch (WebException ex) when (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var statusCode = ((HttpWebResponse)ex.Response).StatusCode;
                    if (statusCode == HttpStatusCode.Unauthorized) throw new MessengerException("Authentication failed", ex);
                    if (statusCode == HttpStatusCode.BadRequest) throw new MessengerException("Invalid request", ex);
                    if (statusCode == HttpStatusCode.Forbidden)
                    {
                        // TODO: Handle policy exceptions
                        throw new MessengerException("Request failed due to policy exception", ex);
                    }

                    throw new MessengerException("Request failed due to unknown web exception", ex);
                }
                catch (Exception ex)
                {
                    throw new MessengerException("Request failed due to unknown reason", ex);
                }

                try
                {
                    var response = Encoding.UTF8.GetString(responseBytes);
                    return JsonConvert.DeserializeObject<DeliveryInfoListContainer>(response);
                }
                catch (Exception ex)
                {
                    throw new MessengerException("Response reading failed due to unknown reason", ex);
                }
            }
        }

        /// <exception cref="AuthenticationException">Thrown when the authentication fails</exception>
        /// <exception cref="MessengerException">Thrown when sending of inbound message request fails</exception>
        public async Task<InboundMessageListContainer> RetrieveAndDeleteMessagesAsync(InboundMessageRetrieveAndDeleteRequestContainer inboundMessage)
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
                    var jsonStr = JsonConvert.SerializeObject(inboundMessage);
                    var jsonBytes = Encoding.UTF8.GetBytes(jsonStr);

                    responseBytes = await client.UploadDataTaskAsync(
                        $"{Config.CombinedAddress}/inbound/registrations/{Config.RegistrationId}/messages/retrieveAndDeleteMessages", "POST", jsonBytes);
                }
                catch (WebException ex) when (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var statusCode = ((HttpWebResponse)ex.Response).StatusCode;
                    if (statusCode == HttpStatusCode.Unauthorized) throw new MessengerException("Authentication failed", ex);
                    if (statusCode == HttpStatusCode.BadRequest) throw new MessengerException("Invalid request", ex);
                    if (statusCode == HttpStatusCode.Forbidden)
                    {
                        // TODO: Handle policy exceptions
                        throw new MessengerException("Request failed due to policy exception", ex);
                    }

                    throw new MessengerException("Request failed due to unknown web exception", ex);
                }
                catch (Exception ex)
                {
                    throw new MessengerException("Request failed due to unknown reason", ex);
                }

                try
                {
                    var response = Encoding.UTF8.GetString(responseBytes);
                    return JsonConvert.DeserializeObject<InboundMessageListContainer>(response);
                }
                catch (Exception ex)
                {
                    throw new MessengerException("Response reading failed due to unknown reason", ex);
                }
            }
        }

        /// <exception cref="AuthenticationException">Thrown when the authentication fails</exception>
        public async Task RefreshAccessTokenIfNeededAsync()
        {
            lock (accessTokenLock)
            {
                if (latestAccessToken != null && (DateTime.UtcNow - accessTokenExpires).TotalSeconds > 10) return;
            }

            await ForceAccessTokenRefreshAsync();
        }

        /// <exception cref="AuthenticationException">Thrown when the authentication fails</exception>
        public async Task ForceAccessTokenRefreshAsync()
        {
            var newAccessToken = await Authenticator.RequestAccessTokenAsync();

            lock (accessTokenLock)
            {
                latestAccessToken = newAccessToken;
                accessTokenExpires = DateTime.UtcNow + TimeSpan.FromSeconds(latestAccessToken.ExpiresIn);
            }
        }
    }
}
