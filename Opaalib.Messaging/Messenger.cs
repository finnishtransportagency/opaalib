using Newtonsoft.Json;
using Opaalib.OAuth;
using Opaalib.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Opaalib.Messaging
{
    public class Messenger
    {
        public Authenticator Authenticator { get; }
        public MessengerConfiguration Config { get; }

        private DateTime accessTokenExpires;
        private AccessTokenResponse latestAccessToken;
        private readonly SemaphoreSlim accessTokenLock = new SemaphoreSlim(1, 1);

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
                    HandleWebException(ex);
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
                    HandleWebException(ex);
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
        public async Task<InboundMessageListContainer> RetrieveAndDeleteMessagesAsync(InboundMessageRetrieveAndDeleteRequestContainer inboundMessage, string registrationId)
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
                        $"{Config.CombinedAddress}/inbound/registrations/{registrationId}/messages/retrieveAndDeleteMessages", "POST", jsonBytes);
                }
                catch (WebException ex) when (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HandleWebException(ex);
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
            await Task.Factory.StartNew(() => accessTokenLock.Wait());

            var expires = (DateTime.UtcNow - accessTokenExpires).TotalSeconds;
            if (latestAccessToken != null && (DateTime.UtcNow - accessTokenExpires).TotalSeconds > 10)
            {
                accessTokenLock.Release();
                return;
            }

            await ForceAccessTokenRefreshAsyncInternal();
            accessTokenLock.Release();
        }

        private async Task ForceAccessTokenRefreshAsyncInternal()
        {
            var newAccessToken = await Authenticator.RequestAccessTokenAsync();

            latestAccessToken = newAccessToken;
            accessTokenExpires = DateTime.UtcNow + TimeSpan.FromSeconds(latestAccessToken.ExpiresIn);
        }

        /// <exception cref="AuthenticationException">Thrown when the authentication fails</exception>
        public async Task ForceAccessTokenRefreshAsync()
        {
            await Task.Factory.StartNew(() => accessTokenLock.Wait());

            await ForceAccessTokenRefreshAsyncInternal();

            accessTokenLock.Release();
        }

        private void HandleWebException(WebException ex)
        {
            var response = (HttpWebResponse)ex.Response;
            var statusCode = response.StatusCode;

            if (statusCode == HttpStatusCode.Unauthorized) throw new MessengerException("Authentication failed", ex);
            if (statusCode == HttpStatusCode.BadRequest) throw new MessengerException("Invalid request", ex);
            if (statusCode == HttpStatusCode.Forbidden)
            {
                // TODO: Handle policy exceptions
                throw new MessengerException("Request failed due to policy exception", ex);
            }

            throw new MessengerException("Request failed due to unknown web exception", ex);
        }
    }
}
