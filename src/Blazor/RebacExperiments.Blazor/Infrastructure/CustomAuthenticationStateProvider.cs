using Microsoft.AspNetCore.Components.Authorization;
using RebacExperiments.Shared.ApiSdk;

namespace RebacExperiments.Blazor.Infrastructure
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ApiClient _apiClient;

        public CustomAuthenticationStateProvider(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return null;   
        }
    }
}
