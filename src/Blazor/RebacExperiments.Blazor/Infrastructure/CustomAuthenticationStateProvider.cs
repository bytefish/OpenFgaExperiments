// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Components.Authorization;
using RebacExperiments.Shared.ApiSdk.Models;
using System.Security.Claims;

namespace RebacExperiments.Blazor.Infrastructure
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private const string LocalStorageKey = "currentUser";

        private readonly LocalStorageService _localStorageService;

        public CustomAuthenticationStateProvider(LocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var currentUser = await GetCurrentUserAsync();

            if(currentUser == null)
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            Claim[] claims = [
                new Claim(ClaimTypes.NameIdentifier, currentUser.Id!.ToString()!),
                new Claim(ClaimTypes.Name, currentUser.LogonName!.ToString()!),
                new Claim(ClaimTypes.Email, currentUser.LogonName!.ToString()!)
            ];

            var authenticationState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType: nameof(CustomAuthenticationStateProvider))));

            return authenticationState;
        }

        public async Task SetCurrentUserAsync(User? currentUser)
        { 
            await _localStorageService.SetItem(LocalStorageKey, currentUser);
            
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public Task<User?> GetCurrentUserAsync() => _localStorageService.GetItemAsync<User>(LocalStorageKey);
    }
}