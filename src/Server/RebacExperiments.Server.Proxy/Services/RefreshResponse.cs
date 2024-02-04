using System.Text.Json.Serialization;

namespace RebacExperiments.Server.Proxy.Services
{
    public class RefreshResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = "";

        [JsonPropertyName("id_token")]
        public string IdToken { get; set; } = "";

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = "";

        [JsonPropertyName("expires")]
        public long Expires { get; set; }
    }
}