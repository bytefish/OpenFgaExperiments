namespace RebacExperiments.Server.Proxy.Services
{
    public class AzureAdTokenExchangeService : ITokenExchangeService
    {
        private readonly ILogger<AzureAdTokenExchangeService> _logger;

        private readonly DiscoveryDocument _disco;
        private readonly GatewayConfig _config;

        public AzureAdTokenExchangeService(ILogger<AzureAdTokenExchangeService> logger, GatewayConfig config, DiscoveryDocument disco)
        {
            _logger = logger;
            _disco = disco;
            _config = config;
        }

        public async Task<TokenExchangeResponse> Exchange(string accessToken, ApiConfig apiConfig)
        {
            var httpClient = new HttpClient();

            var scope = apiConfig.ApiScopes;

            var url = _disco.TokenEndpoint;

            var dict = new Dictionary<string, string>()
            {
                { "grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"},
                { "client_id", _config.ClientId },
                { "client_secret", _config.ClientSecret },
                { "assertion", accessToken },
                { "scope", scope },
                { "requested_token_use", "on_behalf_of" },
            };

            var content = new FormUrlEncodedContent(dict);

            var httpResponse = await httpClient.PostAsync(url, content);

            var response = await httpResponse.Content.ReadFromJsonAsync<TokenExchangeResponse>();

            if (response == null)
            {
                throw new Exception("error exchaning token at " + _disco.TokenEndpoint);
            }

            return response;
        }
    }
}