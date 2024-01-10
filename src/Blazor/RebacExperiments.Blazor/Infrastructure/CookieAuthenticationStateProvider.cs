// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Components.Authorization;
using RebacExperiments.Shared.ApiSdk;
using RebacExperiments.Shared.ApiSdk.Models;
using System.Security.Claims;

namespace RebacExperiments.Blazor.Infrastructure
{
    /// <summary>
    /// This is a pretty simple <see cref="AuthenticationStateProvider" />. We should add 
    /// some kind of expiration. What happens, when the server sends a 401 for the GetMe() 
    /// request?
    /// </summary>
    public class CookieAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ApiClient _apiClient;
        
        private User? _currentUser;
        private ClaimsPrincipal _claimsPrincipal;

        public CookieAuthenticationStateProvider(ApiClient apiClient)
        {
            _apiClient = apiClient;
            _claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (_currentUser == null)
            {
                var user = await _apiClient.Odata.Me.GetAsync();

                if(user != null)
                {
                    var identity = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Email, user.LogonName!),
                        new Claim(ClaimTypes.Name, user.PreferredName!),
                        new Claim("UserID", user.Id!.ToString()!),
                    }, "AuthCookie");

                    _claimsPrincipal = new ClaimsPrincipal(identity);
                }
            }

            return new AuthenticationState(_claimsPrincipal!);
        }
    }
}
