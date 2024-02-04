using RebacExperiments.Server.Proxy.Middleware;

namespace RebacExperiments.Server.Proxy.Services
{
    public class GatewayService
    {
        private readonly ILogger<GatewayService> _logger;

        private readonly TokenRefreshService _tokenRefreshService;
        private readonly GatewayConfig _config;
        private readonly ApiTokenService _apiTokenService;
        
        public GatewayService(ILogger<GatewayService> logger,  TokenRefreshService tokenRefreshService, GatewayConfig config, ApiTokenService apiTokenService)
        {
            _tokenRefreshService = tokenRefreshService;
            _config = config;
            _apiTokenService = apiTokenService;
            _logger = logger;
        }

        public async Task AddToken(HttpContext ctx)
        {
            if (IsExpired(ctx) && HasRefreshToken(ctx))
            {
                _apiTokenService.InvalidateApiTokens(ctx);

                await Refresh(ctx, _tokenRefreshService);
            }

            var token = ctx.Session.GetString(SessionKeys.ACCESS_TOKEN);

            var currentUrl = ctx.Request.Path.ToString().ToLower();

            var apiConfig = _config.ApiConfigs.FirstOrDefault(c => currentUrl.StartsWith(c.ApiPath));

            if (!string.IsNullOrEmpty(token) && apiConfig != null)
            {
                var apiToken = await GetApiToken(ctx, _apiTokenService, token, apiConfig);

                _logger.LogDebug($"---- Adding Token for reqeuest ----\n{currentUrl}\n\n{apiToken}\n--------");

                ctx.Request.Headers.TryAdd("Authorization", "Bearer " + apiToken);
            }
        }

        private bool IsExpired(HttpContext ctx)
        {
            var expiresAt = Convert.ToInt64(ctx.Session.GetString(SessionKeys.EXPIRES_AT)) - 30;
            var now = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();

            var expired = now >= expiresAt;

            return expired;
        }

        private bool HasRefreshToken(HttpContext ctx)
        {
            var refreshToken = ctx.Session.GetString(SessionKeys.REFRESH_TOKEN);

            return !string.IsNullOrEmpty(refreshToken);
        }

        private async Task Refresh(HttpContext ctx, TokenRefreshService tokenRefreshService)
        {
            var refreshToken = GetRefreshToken(ctx);

            var resp = await tokenRefreshService.RefreshAsync(refreshToken);

            if (resp == null)
            {
                // Next call to API will fail with 401 and client can take action
                return;
            }

            var expiresAt = new DateTimeOffset(DateTime.Now).AddSeconds(Convert.ToInt32(resp.Expires));

            ctx.Session.SetString(SessionKeys.ACCESS_TOKEN, resp.AccessToken);
            ctx.Session.SetString(SessionKeys.ID_TOKEN, resp.IdToken);
            ctx.Session.SetString(SessionKeys.REFRESH_TOKEN, resp.RefreshToken);
            ctx.Session.SetString(SessionKeys.EXPIRES_AT, "" + expiresAt.ToUnixTimeSeconds());
        }

        private string GetRefreshToken(HttpContext ctx)
        {
            var refreshToken = ctx.Session.GetString(SessionKeys.REFRESH_TOKEN);

            return refreshToken ?? "";
        }

        private async Task<string> GetApiToken(HttpContext ctx, ApiTokenService apiTokenService, string token, ApiConfig? apiConfig)
        {
            string? apiToken = null;

            if (!string.IsNullOrEmpty(apiConfig?.ApiScopes) || !string.IsNullOrEmpty(apiConfig?.ApiAudience))
            {
                apiToken = await apiTokenService.LookupApiToken(ctx, apiConfig, token);

                ShowDebugMessage(apiToken);
            }

            if (!string.IsNullOrEmpty(apiToken))
            {
                return apiToken;
            }
            else
            {
                return token;
            }
        }

        private void ShowDebugMessage(string? token)
        {
            this._logger.LogDebug($"---- api access_token ----\n{token}\n--------");
        }

    }
}