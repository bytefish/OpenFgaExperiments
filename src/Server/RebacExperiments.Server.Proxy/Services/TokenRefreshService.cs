namespace RebacExperiments.Server.Proxy.Services
{
    public class TokenRefreshService
    {
        private readonly ILogger<TokenRefreshService> _logger;
        private readonly DiscoveryDocument _disco;
        private readonly GatewayConfig _config;

        public TokenRefreshService(ILogger<TokenRefreshService> logger, GatewayConfig config, DiscoveryDocument disco)
        {
            _logger = logger;
            _disco = disco;
            _config = config;
        }

        public async Task<RefreshResponse?> RefreshAsync(string refreshToken)
        {
            var payload = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken },
                { "client_id", _config.ClientId },
                { "client_secret", _config.ClientSecret }
            };

            var httpClient = new HttpClient();

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(_disco.TokenEndpoint),
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(payload)
            };

            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<RefreshResponse>();

            return result;

        }
    }
}