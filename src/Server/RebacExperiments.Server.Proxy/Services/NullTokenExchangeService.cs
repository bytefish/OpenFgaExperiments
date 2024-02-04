namespace RebacExperiments.Server.Proxy.Services
{
    public class NullTokenExchangeService : ITokenExchangeService
    {
        public Task<TokenExchangeResponse> Exchange(string accessToken, ApiConfig apiConfig)
        {
            var result = new TokenExchangeResponse
            {
                AccessToken = "",
                ExpiresIn = 0,
                RefreshToken = "",
            };

            return Task.FromResult(result);
        }
    }
}