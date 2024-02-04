using RebacExperiments.Server.Proxy.Middleware;

namespace RebacExperiments.Server.Proxy.Services
{
    public class ApiTokenService
    {
        private readonly ILogger<ApiTokenService> _logger;

        private readonly ITokenExchangeService _tokenExchangeService;

        public ApiTokenService(ILogger<ApiTokenService> logger, ITokenExchangeService tokenExchangeService)
        {
            _logger = logger;
            _tokenExchangeService = tokenExchangeService;
        }

        public void InvalidateApiTokens(HttpContext ctx)
        {
            ctx.Session.Remove(SessionKeys.API_ACCESS_TOKEN);
        }

        private TokenExchangeResponse? GetCachedApiToken(HttpContext ctx, ApiConfig apiConfig)
        {
            var cache = ctx.Session.GetObject<Dictionary<string, TokenExchangeResponse>>(SessionKeys.API_ACCESS_TOKEN);

            if (cache == null)
            {
                return null;
            }

            if (!cache.ContainsKey(apiConfig.ApiPath))
            {
                return null;
            }

            return cache[apiConfig.ApiPath];
        }

        private void SetCachedApiToken(HttpContext ctx, ApiConfig apiConfig, TokenExchangeResponse response)
        {
            var cache = ctx.Session.GetObject<Dictionary<string, TokenExchangeResponse>>(SessionKeys.API_ACCESS_TOKEN);

            if (cache == null)
            {
                cache = new Dictionary<string, TokenExchangeResponse>();
            }

            cache[apiConfig.ApiPath] = response;

            ctx.Session.SetObject(SessionKeys.API_ACCESS_TOKEN, cache);
        }

        public async Task<string> LookupApiToken(HttpContext ctx, ApiConfig apiConfig, string token)
        {
            var apiToken = GetCachedApiToken(ctx, apiConfig);

            if (apiToken != null)
            {
                return apiToken.AccessToken;
            }

            _logger.LogDebug($"---- Perform Token Exchange for {apiConfig.ApiScopes} ----");

            var response = await _tokenExchangeService.Exchange(token, apiConfig);

            SetCachedApiToken(ctx, apiConfig, response);

            return response.AccessToken;
        }
    }
}