// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Components;
using RebacExperiments.Blazor.Infrastructure;

namespace RebacExperiments.Blazor.Pages
{
    public partial class RedirectExternal
    {
        protected override async Task OnInitializedAsync()
        {
            try
            {
                // ... then get the User Profile ...
                var me = await ApiClient.Odata.Me.GetAsync();

                // ... then set the new User Profile ...
                await AuthStateProvider.SetCurrentUserAsync(me);

                // ... and navigate to the ReturnUrl.
                var navigationUrl = "/";

                NavigationManager.NavigateTo(navigationUrl);
            }
            catch (Exception e)
            {
                ApplicationErrorMessageService.ShowErrorMessage(e);
            }

        }
    }
}