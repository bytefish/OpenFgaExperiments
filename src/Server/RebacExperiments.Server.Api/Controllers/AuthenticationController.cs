// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using RebacExperiments.Server.Api.Infrastructure.Errors;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using RebacExperiments.Server.Api.Services;
using RebacExperiments.Server.Database;
using System.Security.Claims;

namespace RebacExperiments.Server.Api.Controllers
{
    public class AuthenticationController : ODataController
    {
        private readonly ILogger<AuthenticationController> _logger;

        private readonly ApplicationErrorHandler _applicationErrorHandler;

        public AuthenticationController(ILogger<AuthenticationController> logger, ApplicationErrorHandler applicationErrorHandler)
        {
            _logger = logger;
            _applicationErrorHandler = applicationErrorHandler;
        }

        [HttpPost("odata/SignInUser")]
        public async Task<IActionResult> SignInUser([FromServices] ApplicationDbContext context, [FromServices] IUserService userService, [FromBody] ODataActionParameters parameters, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                return _applicationErrorHandler.HandleInvalidModelState(HttpContext, ModelState);
            }

            string username = (string)parameters["username"];
            string password = (string)parameters["password"];
            bool rememberMe = (bool)parameters["rememberMe"];

            try
            {
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
            catch (Exception ex)
            {
                return _applicationErrorHandler.HandleException(HttpContext, ex);
            }
        }

        [HttpPost("odata/SignOutUser")]
        public async Task<IActionResult> SignOutUser()
        {
            _logger.TraceMethodEntry();

            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            } 
            catch(Exception ex)
            {
                return _applicationErrorHandler.HandleException(HttpContext, ex);
            }

            return Ok();
        }
    }
}