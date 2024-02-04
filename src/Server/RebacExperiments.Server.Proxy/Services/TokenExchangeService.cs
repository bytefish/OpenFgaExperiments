namespace RebacExperiments.Server.Proxy.Services
{
    public class TokenExchangeService : ITokenExchangeService
    {
        private readonly ILogger<TokenExchangeService> _logger;
        private readonly DiscoveryDocument _disco;
        private readonly GatewayConfig _config;

        public TokenExchangeService(ILogger<TokenExchangeService> logger, GatewayConfig config, DiscoveryDocument disco)
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
                { "grant_type",  "urn:ietf:params:oauth:grant-type:token-exchange" },
                { "client_id", _config.ClientId },
                { "client_secret", _config.ClientSecret },
                { "subject_token", accessToken },
                { "scope", scope },
                { "audience", apiConfig.ApiAudience },
                { "requested_token_type", "urn:ietf:params:oauth:token-type:refresh_token" }
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