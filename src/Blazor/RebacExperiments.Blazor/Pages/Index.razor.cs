// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RebacExperiments.Shared.ApiSdk.Odata.SignInUser;

namespace RebacExperiments.Blazor.Pages
{
    public partial class Index
    {
        public Index()
        {
            
        }

        public async Task SignInUser_Philipp()
        {
            await ApiClient.Odata.SignInUser
                .PostAsync(new SignInUserPostRequestBody
                {
                    Username = "philipp@bytefish.de",
                    Password = "5!F25GbKwU3P",
                    RememberMe = true
                });
        }

        public async Task SignInUser_Max()
        {
            await ApiClient.Odata.SignInUser
                .PostAsync(new SignInUserPostRequestBody
                {
                    Username = "max@mustermann.local",
                    Password = "5!F25GbKwU3P",
                    RememberMe = true
                });
        }

        public async Task SignOutUser()
        {
            await ApiClient.Odata.SignOutUser.PostAsync();
        }
    }
}
