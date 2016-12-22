using Newtonsoft.Json;

namespace SoneraOMA.OAuth
{
    public class AccessTokenResponse
    {
        [JsonProperty(PropertyName = "access_token", Required = Required.Always)]
        public string AccessToken { get; set; }

        [JsonProperty(PropertyName = "token_type", Required = Required.Always)]
        public string TokenType { get; set; }

        [JsonProperty(PropertyName = "expires_in", Required = Required.Always)]
        public int ExpiresIn { get; set; }
    }

    public enum AccessTokenError
    {
        RequestFailed,
        InvalidRequest,
        InvalidResponse,
        AuthenticationFailed
    }
}