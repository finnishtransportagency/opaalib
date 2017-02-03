using Newtonsoft.Json;
using Opaalib.OAuth;
using Opaalib.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
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

        public Messenger(Authenticator authenticator)
            : this(authenticator, MessengerConfiguration.Default)
        {
            
        }

        /// <exception cref="AuthenticationException">Thrown when the authentication fails</exception>
        /// <exception cref="MessengerException">Thrown when sending of outbound message request fails</exception>
        public async Task<OutboundMessageResponse> OutboundMessageRequestAsync(OutboundMessageRequest outboundMessage)
        {
            await RefreshAccessTokenIfNeededAsync();

            using (var client = new MyWebClient())
            {
                client.Headers[HttpRequestHeader.Authorization] = $"Bearer {latestAccessToken.AccessToken}";
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.Headers[HttpRequestHeader.Accept] = "application/json";
                byte[] responseBytes = null;
                try
                {
                    var finalOutboundMessage = new OutboundMessageRequestContainer { OutboundMessageRequest = outboundMessage };
                    var jsonStr = JsonConvert.SerializeObject(finalOutboundMessage);
                    var jsonBytes = Encoding.UTF8.GetBytes(jsonStr);

                    responseBytes = await client.UploadDataTaskAsync(
                        $"{Config.BaseAddress}/outbound/{outboundMessage.SenderAddress}/requests", "POST", jsonBytes);
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

        public async Task<DeliveryInfoList> ReadOutboundMessageDeliveryStatusAsync(string outboundResourceUrl)
        {
            await RefreshAccessTokenIfNeededAsync();

            using (var client = new MyWebClient())
            {
                client.Headers[HttpRequestHeader.Authorization] = $"Bearer {latestAccessToken.AccessToken}";
                client.Headers[HttpRequestHeader.Accept] = "application/json";
                byte[] responseBytes = null;
                try
                {
                    responseBytes = await client.DownloadDataTaskAsync($"{outboundResourceUrl}/deliveryInfos");

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
                    var jsonObj = JsonConvert.DeserializeObject<DeliveryInfoListContainer>(response);
                    return jsonObj.DeliveryInfoList;
                }
                catch (Exception ex)
                {
                    throw new MessengerException("Response reading failed due to unknown reason", ex);
                }
            }
        }

        /// <exception cref="AuthenticationException">Thrown when the authentication fails</exception>
        /// <exception cref="MessengerException">Thrown when reading delivery status fails</exception>
        public async Task<DeliveryInfoList> ReadOutboundMessageDeliveryStatusAsync(string requestId, string senderAddress)
        {
            return await ReadOutboundMessageDeliveryStatusAsync($"{Config.BaseAddress}/outbound/{senderAddress}/requests/{requestId}");
        }

        /// <exception cref="AuthenticationException">Thrown when the authentication fails</exception>
        /// <exception cref="MessengerException">Thrown when sending of inbound message request fails</exception>
        public async Task<InboundMessageList> RetrieveAndDeleteMessagesAsync(InboundMessageRetrieveAndDeleteRequest inboundMessage, string registrationId)
        {
            await RefreshAccessTokenIfNeededAsync();

            using (var client = new MyWebClient())
            {
                client.Headers[HttpRequestHeader.Authorization] = $"Bearer {latestAccessToken.AccessToken}";
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.Headers[HttpRequestHeader.Accept] = "application/json";
                byte[] responseBytes = null;
                try
                {
                    var finalInboundMessage = new InboundMessageRetrieveAndDeleteRequestContainer { InboundMessageRetrieveAndDeleteRequest = inboundMessage };
                    var jsonStr = JsonConvert.SerializeObject(finalInboundMessage);
                    var jsonBytes = Encoding.UTF8.GetBytes(jsonStr);

                    responseBytes = await client.UploadDataTaskAsync(
                        $"{Config.BaseAddress}/inbound/registrations/{registrationId}/messages/retrieveAndDeleteMessages", "POST", jsonBytes);
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
                    var jsonObj = JsonConvert.DeserializeObject<InboundMessageListContainer>(response);
                    return jsonObj.InboundMessageList;
                }
                catch (Exception ex)
                {
                    throw new MessengerException("Response reading failed due to unknown reason", ex);
                }
            }
        }

        public IDisposable StartReceivingNotifications(
            Uri inboundMessageBaseUri, IObserver<InboundMessageNotification> inboundMessageObserver,
            Uri outboundMessageStatusBaseUri, IObserver<DeliveryInfoNotification> outboundMessageStatusObserver)
        {
            // parameters have to be either both null or neither null
            if ((inboundMessageBaseUri == null) != (inboundMessageObserver == null))
            {
                throw new ArgumentException($"{nameof(inboundMessageBaseUri)} and {nameof(inboundMessageObserver)} have to be either both not null or both null");
            }
            if ((outboundMessageStatusBaseUri == null) != (outboundMessageStatusObserver == null))
            {
                throw new ArgumentException($"{nameof(outboundMessageStatusBaseUri)} and {nameof(outboundMessageStatusObserver)} have to be either both not null or both null");
            }
            if (inboundMessageBaseUri == null && outboundMessageStatusBaseUri == null)
            {
                throw new ArgumentException($"Both {nameof(inboundMessageBaseUri)} and {nameof(outboundMessageStatusBaseUri)} can't be null");
            }

            var sameUris = inboundMessageBaseUri == outboundMessageStatusBaseUri;

            var listener = new HttpListener();
            if (inboundMessageBaseUri != null) listener.Prefixes.Add(inboundMessageBaseUri.AbsoluteUri);
            if (outboundMessageStatusBaseUri != null) listener.Prefixes.Add(outboundMessageStatusBaseUri.AbsoluteUri);
            listener.Start();

            Subject<InboundMessageNotification> inboundMessageSubject = null;
            if (inboundMessageObserver != null)
            {
                inboundMessageSubject = new Subject<InboundMessageNotification>();
                inboundMessageSubject.Subscribe(inboundMessageObserver);
            }
            Subject<DeliveryInfoNotification> outboundMessageStatusSubject = null;
            if (outboundMessageStatusObserver != null)
            {
                outboundMessageStatusSubject = new Subject<DeliveryInfoNotification>();
                outboundMessageStatusSubject.Subscribe(outboundMessageStatusObserver);
            }

            var cts = new CancellationTokenSource();
            var token = cts.Token;

            TaskEx.Run(async () =>
            {
                while (true)
                {
                    if (token.IsCancellationRequested) return;

                    var context = await listener.GetContextAsync();

                    var request = context.Request;
                    var response = context.Response;

                    if (token.IsCancellationRequested)
                    {
                        // this is in try so that if we somehow get here after listener has been already closed we don't crash
                        try
                        {
                            response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            response.Close();
                        }
                        catch (Exception)
                        {
                            
                        }

                        return;
                    }

                    if (request.HttpMethod != "POST")
                    {
                        response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                        response.Close();
                        continue;
                    }

                    if (request.ContentType != "application/json")
                    {
                        response.StatusCode = (int)HttpStatusCode.UnsupportedMediaType;
                        response.Close();
                        continue;
                    }

                    string str = null;
                    try
                    {
                        using (TextReader tr = new StreamReader(request.InputStream, request.ContentEncoding))
                        {
                            str = await tr.ReadToEndAsync();
                        }
                    }
                    catch (Exception)
                    {
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        response.Close();
                        continue;
                    }

                    if (inboundMessageBaseUri != null && request.Url.AbsolutePath.TrimEnd('/').EndsWith(inboundMessageBaseUri.AbsolutePath.TrimEnd('/')))
                    {
                        InboundMessageNotification requestObject = null;
                        try
                        {
                            var tempRequestObject = JsonConvert.DeserializeObject<InboundMessageNotificationContainer>(str);
                            requestObject = tempRequestObject.InboundMessageNotification;
                        }
                        catch (Exception)
                        {
                            // if the uris are same we need to check which JSON object it is by trying to deserialize to both objects
                            // so having same uris makes this whole thing slower
                            if (sameUris) goto sameUriContinue;

                            response.StatusCode = (int)HttpStatusCode.BadRequest;
                            response.Close();
                            continue;
                        }

                        response.StatusCode = (int)HttpStatusCode.NoContent;
                        response.Close();

                        inboundMessageSubject.OnNext(requestObject);
                        continue;
                    }

                    sameUriContinue:
                    if (outboundMessageStatusBaseUri != null && request.Url.AbsolutePath.TrimEnd('/').EndsWith(outboundMessageStatusBaseUri.AbsolutePath.TrimEnd('/')))
                    {
                        DeliveryInfoNotification requestObject = null;
                        try
                        {
                            var tempRequestObject = JsonConvert.DeserializeObject<DeliveryInfoNotificationContainer>(str);
                            requestObject = tempRequestObject.DeliveryInfoNotification;
                        }
                        catch (Exception)
                        {
                            response.StatusCode = (int)HttpStatusCode.BadRequest;
                            response.Close();
                            continue;
                        }

                        response.StatusCode = (int)HttpStatusCode.NoContent;
                        response.Close();

                        outboundMessageStatusSubject.OnNext(requestObject);
                        continue;
                    }
                }
            }, token);
            
            return Disposable.Create(() =>
            {
                cts.Cancel();
                listener.Stop();
                listener.Close();
                inboundMessageSubject.Dispose();
            });
        }

        /// <exception cref="AuthenticationException">Thrown when the authentication fails</exception>
        public async Task RefreshAccessTokenIfNeededAsync()
        {
            await TaskEx.Run(() => accessTokenLock.Wait());

            var expires = (DateTime.UtcNow - accessTokenExpires).TotalSeconds;
            if (latestAccessToken != null && (DateTime.UtcNow - accessTokenExpires).TotalSeconds > 10)
            {
                accessTokenLock.Release();
                return;
            }

            await ForceAccessTokenRefreshAsyncInternal();
        }

        private async Task ForceAccessTokenRefreshAsyncInternal()
        {
            AccessTokenResponse newAccessToken = null;
            try
            {
                newAccessToken = await Authenticator.RequestAccessTokenAsync();

                latestAccessToken = newAccessToken;
                accessTokenExpires = DateTime.UtcNow + TimeSpan.FromSeconds(latestAccessToken.ExpiresIn);
            }
            finally
            {
                accessTokenLock.Release();
            }
        }

        /// <exception cref="AuthenticationException">Thrown when the authentication fails</exception>
        public async Task ForceAccessTokenRefreshAsync()
        {
            await TaskEx.Run(() => accessTokenLock.Wait());
            await ForceAccessTokenRefreshAsyncInternal();
        }

        private void HandleWebException(WebException ex)
        {
            var response = (HttpWebResponse)ex.Response;
            var statusCode = response.StatusCode;

            if (statusCode == HttpStatusCode.Unauthorized) throw new MessengerException("Authentication failed", ex);
            if (statusCode == HttpStatusCode.BadRequest) throw new MessengerException("Invalid request", ex);
            if (statusCode == HttpStatusCode.Forbidden)
            {
                throw new MessengerException("Request failed due to policy exception", ex);
            }

            throw new MessengerException("Request failed due to unknown web exception", ex);
        }
    }
}
