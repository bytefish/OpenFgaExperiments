// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AspNet.Security.OAuth.GitHub;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData;
using RebacExperiments.Server.Api.Infrastructure.Authentication;
using RebacExperiments.Server.Api.Infrastructure.Errors;
using RebacExperiments.Server.Api.Infrastructure.Exceptions;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using RebacExperiments.Server.Api.Services;
using RTools_NTS.Util;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading;

namespace RebacExperiments.Server.Api.Controllers
{
    public class AuthenticationController : ODataController
    {
        private readonly ILogger<AuthenticationController> _logger;

        private readonly ExceptionToODataErrorMapper _exceptionToODataErrorMapper;

        public AuthenticationController(ILogger<AuthenticationController> logger, ExceptionToODataErrorMapper exceptionToODataErrorMapper)
        {
            _logger = logger;
            _exceptionToODataErrorMapper = exceptionToODataErrorMapper;
        }

        [HttpPost("odata/SignInUser")]
        public async Task<IActionResult> SignInUser([FromServices] IUserService userService, [FromBody] ODataActionParameters parameters, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            try
            {
                if (!ModelState.IsValid)
                {
                    throw new InvalidModelStateException
                    {
                        ModelStateDictionary = ModelState
                    };
                }

                string username = (string)parameters["username"];
                string password = (string)parameters["password"];
                bool rememberMe = (bool)parameters["rememberMe"];

                // Create ClaimsPrincipal from Database 
                var userClaims = await userService.GetClaimsAsync(
                    username: username,
                    password: password,
                    cancellationToken: cancellationToken);

                // Create the ClaimsPrincipal
                var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                // It's a valid ClaimsPrincipal, sign in
                await HttpContext.SignInAsync(claimsPrincipal, new AuthenticationProperties { IsPersistent = rememberMe });

                return Ok();
            }
            catch (Exception exception)
            {
                return _exceptionToODataErrorMapper.CreateODataErrorResult(HttpContext, exception);
            }
        }
        
        [HttpGet("odata/login.github(redirectUrl={redirectUrl})")]
        public IActionResult GitHubLogin([FromODataUri] string redirectUrl)
        {
            _logger.TraceMethodEntry();

            try
            {
                if (!ModelState.IsValid)
                {
                    throw new InvalidModelStateException
                    {
                        ModelStateDictionary = ModelState
                    };
                }

                // Encode the Redirect URL again, because the GitHub OAuth Server 
                // calls this endpoint, when the Authentication is done.
                redirectUrl = UrlEncoder.Default.Encode(redirectUrl);

                var authenticationProperties = new AuthenticationProperties
                {
                    RedirectUri = $"/odata/signin.github(redirectUrl='{redirectUrl}')",
                };

                return Challenge(authenticationProperties, GitHubAuthenticationDefaults.AuthenticationScheme);
            }
            catch (Exception exception)
            {
                return _exceptionToODataErrorMapper.CreateODataErrorResult(HttpContext, exception);
            }
        }

        [HttpGet("odata/signin.github(redirectUrl={redirectUrl})")]
        public async Task<IActionResult> GitHubSignIn([FromServices] IUserService userService, [FromODataUri] string redirectUrl, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            try
            {
                if (!ModelState.IsValid)
                {
                    throw new InvalidModelStateException
                    {
                        ModelStateDictionary = ModelState
                    };
                }      

                // Evaluates the Response GitHub has sent to us. This basically resolves 
                // the Authentication Scheme we have registered in the Program.cs and
                // evaluates the response.
                var result = await HttpContext.AuthenticateAsync(AuthenticationSchemes.ExternalScheme);

                // If the OAuth Response was not successful, we stop here and throw.
                if (!result.Succeeded)
                {
                    throw new AuthenticationFailedException("GitHub Login failed", result.Failure);
                }

                // In this application we are using an E-Mail as Identification. You may 
                // also use the NameIdentifier Claim or anything else you need to resolve 
                // the claims in your backend.
                var username = result.Principal.FindFirstValue(ClaimTypes.Email);

                // Create ClaimsPrincipal from our local database.
                if (string.IsNullOrWhiteSpace(username))
                {
                    throw new AuthenticationFailedException("GitHub Login failed");
                }

                var userClaims = await userService.GetClaimsAsync(username: username, cancellationToken);
                var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                // Build the Authentication Properties.
                var authenticationProperties = new AuthenticationProperties()
                {
                    IsPersistent = true
                };

                // This signals, that we authenticated the user against an external provider.
                authenticationProperties.SetString("ExternalProviderName", GitHubAuthenticationDefaults.AuthenticationScheme);

                // If we have received a token, we can add it to the Cookie.
                var accessToken = await HttpContext
                    .GetTokenAsync("access_token")
                    .ConfigureAwait(false);

                if (accessToken != null)
                {
                    var tokens = new[]
                    {
                        new AuthenticationToken
                        {
                            Name = "access_token",
                            Value = accessToken
                        }
                    };

                    authenticationProperties.StoreTokens(tokens);
                }

                // Perform the SignIn, so we set the Authentication Cookie.
                await HttpContext.SignInAsync(claimsPrincipal, authenticationProperties);

                // Delete all Correlation Cookies set during the OAuth Dance.
                await HttpContext.SignOutAsync(AuthenticationSchemes.ExternalScheme);

                

                redirectUrl = Uri.UnescapeDataString(redirectUrl);

                return Redirect(redirectUrl);
            }
            catch (Exception exception)
            {
                return _exceptionToODataErrorMapper.CreateODataErrorResult(HttpContext, exception);
            }
        }

        [HttpPost("odata/SignOutUser")]
        public async Task<IActionResult> SignOutUser()
        {
            _logger.TraceMethodEntry();

            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                return Ok();
            }
            catch (Exception exception)
            {
                return _exceptionToODataErrorMapper.CreateODataErrorResult(HttpContext, exception);
            }
        }
    }
}