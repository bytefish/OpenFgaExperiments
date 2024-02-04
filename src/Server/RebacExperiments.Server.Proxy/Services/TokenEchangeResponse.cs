using System.Text.Json.Serialization;

namespace RebacExperiments.Server.Proxy.Services
{
    public class TokenExchangeResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = "";

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = "";

        [JsonPropertyName("expires_in")]
        public long ExpiresIn { get; set; }
    }
}