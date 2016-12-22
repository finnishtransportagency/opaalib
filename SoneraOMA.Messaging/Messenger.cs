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

        public async Task<Result<OutboundMessageResponse, OutboundMessageError>> OutboundMessageRequestAsync(OutboundMessageRequestContainer outboundMessage)
        {
            Result<OutboundMessageResponse, OutboundMessageError> CreateFailure(OutboundMessageError error) =>
                Result.CreateFailure<OutboundMessageResponse, OutboundMessageError>(error);

            var validToken = await RefreshAccessTokenIfNeededAsync();
            if (!validToken) return CreateFailure(OutboundMessageError.AuthenticationFailed);

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
                    if (statusCode == HttpStatusCode.Unauthorized) return CreateFailure(OutboundMessageError.AuthenticationFailed);
                    if (statusCode == HttpStatusCode.BadRequest) return CreateFailure(OutboundMessageError.InvalidRequest);
                    if (statusCode == HttpStatusCode.Forbidden)
                    {
                        // TODO: Handle policy exceptions
                        return CreateFailure(OutboundMessageError.RequestFailed);
                    }

                    return CreateFailure(OutboundMessageError.RequestFailed);
                }
                catch (Exception)
                {
                    return CreateFailure(OutboundMessageError.RequestFailed);
                }

                try
                {
                    var response = Encoding.UTF8.GetString(responseBytes);
                    return Result.CreateSuccess<OutboundMessageResponse, OutboundMessageError>(
                        JsonConvert.DeserializeObject<OutboundMessageResponse>(response));
                }
                catch (Exception)
                {
                    return CreateFailure(OutboundMessageError.InvalidResponse);
                }
            }
        }

        public async Task<bool> RefreshAccessTokenIfNeededAsync()
        {
            if (latestAccessToken != null && (DateTime.UtcNow - accessTokenExpires).TotalSeconds > 10) return true;

            var result = await Authenticator.RequestAccessTokenAsync();
            if (result.IsOk)
            {
                latestAccessToken = result.GetResult();
                accessTokenExpires = DateTime.UtcNow + TimeSpan.FromSeconds(latestAccessToken.ExpiresIn);
                return true;
            }

            return false;
        }
    }
}
