// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Components.WebAssembly.Http;
using RebacExperiments.Blazor.Shared.Logging;

namespace RebacExperiments.Blazor.Infrastructure
{
    public class CookieDelegatingHandler : DelegatingHandler
    {
        private readonly ILogger<CookieDelegatingHandler> _logger;

        public CookieDelegatingHandler(ILogger<CookieDelegatingHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
