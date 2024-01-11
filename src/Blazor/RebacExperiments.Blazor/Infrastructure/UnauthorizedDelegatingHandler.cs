// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Components;
using RebacExperiments.Blazor.Shared.Logging;

namespace RebacExperiments.Blazor.Infrastructure
{
    public class UnauthorizedDelegatingHandler : DelegatingHandler
    {
        private readonly ILogger<UnauthorizedDelegatingHandler> _logger;

        private readonly CustomAuthenticationStateProvider _customAuthenticationStateProvider;
        private readonly NavigationManager _navigationManager;

        public UnauthorizedDelegatingHandler(ILogger<UnauthorizedDelegatingHandler> logger, CustomAuthenticationStateProvider customAuthenticationStateProvider, NavigationManager navigationManager)
        {
            _logger = logger;
            _customAuthenticationStateProvider = customAuthenticationStateProvider;
            _navigationManager = navigationManager;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await _customAuthenticationStateProvider.SetCurrentUserAsync(null); // Clears the Current User and should trigger a Login
            }

            return response;
        }
    }
}
