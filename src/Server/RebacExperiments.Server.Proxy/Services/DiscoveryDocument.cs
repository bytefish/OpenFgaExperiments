using System.Text.Json.Serialization;

namespace RebacExperiments.Server.Proxy.Services
{
    public class DiscoveryDocument
    {
        [JsonPropertyName("token_endpoint")]
        public string TokenEndpoint { get; set; } = "";
    }
}