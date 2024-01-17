// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Http.HttpClientLibrary;
using RebacExperiments.Shared.ApiSdk.Models.ODataErrors;
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

        public async Task SendInvalidRequest()
        {
            var client = httpClientFactory.CreateClient(nameof(IRequestAdapter));
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "/not-exist");

            httpRequestMessage.Headers.TryAddWithoutValidation("Accept", "application/json;odata.metadata=minimal");

            var res = await client.SendAsync(httpRequestMessage);
        }

        public Task ThrowSystemException()
        {
            throw new Exception("A System.Exception to be caught ...");
        }

        public Task ThrowODataError()
        {
            throw new ODataError
            {
                Error = new MainError
                {
                    Code = "ApiError_RateLimit_000001",
                    Message = "My Message",
                    Innererror = new InnerError
                    {
                        AdditionalData =
                        {
                            { "trace-id", "my-trace-id" }
                        }
                    }
                },
                
            };
        }
    }
}
