// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Components;
using RebacExperiments.Shared.ApiSdk.Odata.SignInUser;
using System.ComponentModel.DataAnnotations;
using static System.Net.WebRequestMethods;
using System.Text.Json.Nodes;
using Microsoft.Kiota.Abstractions;
using RebacExperiments.Blazor.Infrastructure;
using Microsoft.Extensions.Localization;

namespace RebacExperiments.Blazor.Pages
{
    public partial class Login
    {
        // Default Values.
        private class Defaults
        {
            public const string Email = "philipp@bytefish.de";

            public const string Password = "5!F25GbKwU3P";

            public const bool RememberMe = true;
        }

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
            public string Email { get; set; } = "";

            /// <summary>
            /// Gets or sets the Password.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = "";

            /// <summary>
            /// Gets or sets the RememberMe Flag.
            /// </summary>
            [Required]
            public bool RememberMe { get; set; } = false;
        }

        /// <summary>
        /// The Model the Form is going to bind to.
        /// </summary>
        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = new()
        {
            Email = Defaults.Email,
            Password = Defaults.Password,
            RememberMe = Defaults.RememberMe
        };

        /// <summary>
        /// Error Message.
        /// </summary>
        private string ErrorMessage = string.Empty;

        /// <summary>
        /// Signs in the User to the Service using Cookie Authentication.
        /// </summary>
        /// <returns></returns>
        public async Task SignInUserAsync()
        {
            try
            {
                await ApiClient.Odata.SignInUser.PostAsync(new SignInUserPostRequestBody
                {
                    Username = Input.Email,
                    Password = Input.Password,
                    RememberMe = true
                });

                // Now refresh the Authentication State:
                var me = await ApiClient.Odata.Me.GetAsync();

                await AuthStateProvider.SetCurrentUser(me);

                NavigationManager.NavigateTo("/");
            }
            catch
            {
                // TODO
                ErrorMessage = Loc["Login_Failed"];

                await AuthStateProvider.SetCurrentUser(null);
            }
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

            if (model.RememberMe == null)
            {
                yield return new ValidationError
                {
                    PropertyName = nameof(model.RememberMe),
                    ErrorMessage = Loc.GetString("Validation_IsRequired", nameof(model.RememberMe))
                };
            }
        }
    }
}