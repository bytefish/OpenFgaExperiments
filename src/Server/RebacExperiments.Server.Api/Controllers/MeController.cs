// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.RateLimiting;
using RebacExperiments.Server.Api.Infrastructure.Authentication;
using RebacExperiments.Server.Api.Infrastructure.Constants;
using RebacExperiments.Server.Api.Infrastructure.Errors;
using RebacExperiments.Server.Api.Infrastructure.Exceptions;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using RebacExperiments.Server.Api.Services;

namespace RebacExperiments.Server.Api.Controllers
{
    public class MeController : ODataController
    {
        private readonly ILogger<UsersController> _logger;

        private readonly ODataErrorMapper _odataErrorMapper;

        public MeController(ILogger<UsersController> logger, ODataErrorMapper odataErrorMapper)
        {
            _logger = logger;
            _odataErrorMapper = odataErrorMapper;
        }

        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public async Task<IActionResult> Get([FromServices] IUserService userService, CancellationToken cancellationToken)
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

                // Get the User ID extracted by the Authentication Middleware:
                var meUserId = User.GetUserId();

                var user = await userService.GetUserByIdAsync(meUserId, meUserId, cancellationToken);

                return Ok(user);
            }
            catch (Exception exception)
            {
                return _odataErrorMapper.CreateODataErrorResult(HttpContext, exception);
            }
        }
    }
}