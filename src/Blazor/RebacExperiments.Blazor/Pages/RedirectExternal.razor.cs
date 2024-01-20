// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Components;
using RebacExperiments.Blazor.Infrastructure;

namespace RebacExperiments.Blazor.Pages
{
    public partial class RedirectExternal
    {
        [SupplyParameterFromQuery(Name = "returnUrl")]
        public string? ReturnUrl { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var me = await ApiClient.Odata.Me.GetAsync();

                await AuthStateProvider.SetCurrentUserAsync(me);

                var navigationUrl = GetNavigationUrl();

                NavigationManager.NavigateTo(navigationUrl);
            }
            catch (Exception e)
            {
                ApplicationErrorMessageService.ShowErrorMessage(e);
            }

        }

        private string GetNavigationUrl()
        {
            if (string.IsNullOrWhiteSpace(ReturnUrl))
            {
                return "/";
            }

            return ReturnUrl;
        }
    }
}