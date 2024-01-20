// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Components;
using RebacExperiments.Shared.ApiSdk.Odata.SignInUser;
using System.ComponentModel.DataAnnotations;
using RebacExperiments.Blazor.Infrastructure;
using Microsoft.Extensions.Localization;
using System.Text.Encodings.Web;
using System.Collections.ObjectModel;

namespace RebacExperiments.Blazor.Pages
{
    public partial class Login
    {
        /// <summary>
        /// Data Model for binding to the Form.
        /// </summary>
        private sealed class InputModel
        {
            /// <summary>
            /// Gets or sets the Email.
            /// </summary>
            [Required]
            [EmailAddress]
            public required string Email { get; set; }

            /// <summary>
            /// Gets or sets the Password.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public required string Password { get; set; }

            /// <summary>
            /// Gets or sets the RememberMe Flag.
            /// </summary>
            [Required]
            public bool RememberMe { get; set; } = false;
        }

        // Default Values.
        private static class Defaults
        {
            public static class Philipp
            {
                public const string Email = "philipp@bytefish.de";
                public const string Password = "5!F25GbKwU3P";
                public const bool RememberMe = true;
            }

            public static class MaxMustermann
            {
                public const string Email = "max@mustermann.local";
                public const string Password = "5!F25GbKwU3P";
                public const bool RememberMe = true;
            }
        }


        /// <summary>
        /// If a Return URL is given, we will navigate there after login.
        /// </summary>
        [SupplyParameterFromQuery(Name = "returnUrl")]
        private string? ReturnUrl { get; set; }

        /// <summary>
        /// Logs in with Github.
        /// </summary>
        private string LoginGitHubUrl = "https://localhost:5000/odata/login.github(returnUrl='')";

        /// <summary>
        /// The Model the Form is going to bind to.
        /// </summary>
        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = new()
        {
            Email = Defaults.Philipp.Email,
            Password = Defaults.Philipp.Password,
            RememberMe = Defaults.Philipp.RememberMe
        };

        protected override void OnParametersSet()
        {
            // We need to navigate to the RedirectExternal, after the OAuth Dance. This is 
            // needed, because this Page sets the User in the CustomAuthenticationStore.
            var redirectExternalUri = NavigationManager.ToAbsoluteUri("RedirectExternal");

            // After the Login is done, we want to navigate to the Page the user originally 
            // came from. We need to encode this in the Url.
            var parameters = new Dictionary<string, object?>()
            {
                { "returnUrl", ReturnUrl}
            };

            // The Backend needs to redirect to the page "http://<host>:<port>/RedirectExternal?returnUrl=<returnUrl>", let's 
            // construct the Url with the Query Parameters.
            var redirectUrl = NavigationManager.GetUriWithQueryParameters(redirectExternalUri.AbsoluteUri, parameters.AsReadOnly());

            // We send this Redirect URI as parameter to the login.github method. This will encode 
            // the given Redirect URI, so it can be safely used in the Authentication dance.
            var encodedRedirectUrl = UrlEncoder.Default.Encode(redirectUrl);

            // Finally set the GitHub URL for the Button. We need to resolve the Backend URL from somewhere
            // else probably...
            LoginGitHubUrl = $"https://localhost:5000/odata/login.github(redirectUrl='{encodedRedirectUrl}')";
        }

        /// <summary>
        /// Signs in the User to the Service using Cookie Authentication.
        /// </summary>
        /// <returns></returns>
        public async Task SignInUserAsync()
        {
            try
            {
                // Sign in the User, which sets the Auth Cookie ...
                await ApiClient.Odata.SignInUser.PostAsync(new SignInUserPostRequestBody
                {
                    Username = Input.Email,
                    Password = Input.Password,
                    RememberMe = true
                });

                // ... then get the User Profile ...
                var me = await ApiClient.Odata.Me.GetAsync();

                // ... then set the new User Profile ...
                await AuthStateProvider.SetCurrentUserAsync(me);

                // ... and navigate to the ReturnUrl.
                var navigationUrl = GetNavigationUrl();

                NavigationManager.NavigateTo(navigationUrl);
            }
            catch(Exception e)
            {
                ApplicationErrorMessageService.ShowErrorMessage(e);
            }
        }


        private string GetNavigationUrl()
        {
            if(string.IsNullOrWhiteSpace(ReturnUrl))
            {
                return "/";
            }

            return ReturnUrl;
        }

        /// <summary>
        /// Validates an <see cref="InputModel"/>.
        /// </summary>
        /// <param name="model">InputModel to validate</param>
        /// <returns>The list of validation errors for the EditContext model fields</returns>
        private IEnumerable<ValidationError> ValidateInputModel(InputModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                yield return new ValidationError
                {
                    PropertyName = nameof(model.Email),
                    ErrorMessage = Loc.GetString("Validation_IsRequired", nameof(model.Email))
                };
            }

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                yield return new ValidationError
                {
                    PropertyName = nameof(model.Password),
                    ErrorMessage = Loc.GetString("Validation_IsRequired", nameof(model.Password))
                };
            }
        }
    }
}