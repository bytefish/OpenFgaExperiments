// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.RateLimiting;
using RebacExperiments.Server.Api.Infrastructure.Authentication;
using RebacExperiments.Server.Api.Infrastructure.Constants;
using RebacExperiments.Server.Api.Infrastructure.Exceptions;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using RebacExperiments.Server.Api.Services;
using RebacExperiments.Server.Database.Models;

namespace RebacExperiments.Server.Api.Controllers
{
    public class UsersController : ODataController
    {
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }

        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public async Task<IActionResult> GetUser([FromServices] IUserService userService, [FromODataUri(Name = "key")] int key, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                throw new InvalidModelStateException
                {
                    ModelStateDictionary = ModelState
                };
            }

            var user = await userService.GetUserByIdAsync(key, User.GetUserId(), cancellationToken);

            return Ok(user);
        }

        [HttpGet]
        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public async Task<IActionResult> GetUsers([FromServices] IUserService userService, ODataQueryOptions<TaskItem> queryOptions, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                throw new InvalidModelStateException
                {
                    ModelStateDictionary = ModelState
                };
            }

            var users = await userService.GetUsersByUserIdAsync(User.GetUserId(), cancellationToken);

            return Ok(queryOptions.ApplyTo(users.AsQueryable()));
        }

        [HttpPost]
        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public async Task<IActionResult> PostUser([FromServices] IUserService userService, [FromBody] User user, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                throw new InvalidModelStateException
                {
                    ModelStateDictionary = ModelState
                };
            }

            await userService.CreateUserAsync(user, User.GetUserId(), cancellationToken);

            return Created(user);
        }

        [HttpPut]
        [HttpPatch]
        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public async Task<IActionResult> PatchUser([FromServices] IUserService userService, [FromODataUri] int key, [FromBody] Delta<User> delta, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                throw new InvalidModelStateException
                {
                    ModelStateDictionary = ModelState
                };
            }

            var user = await userService.GetUserByIdAsync(key, User.GetUserId(), cancellationToken);

            // Patch the Values to it:
            delta.Patch(user);

            // Update the Values:
            await userService.UpdateUserAsync(user, User.GetUserId(), cancellationToken);

            return Updated(user);
        }

        [HttpDelete]
        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public async Task<IActionResult> DeleteUser([FromServices] IUserService userService, [FromODataUri(Name = "key")] int key, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                throw new InvalidModelStateException
                {
                    ModelStateDictionary = ModelState
                };
            }

            await userService.DeleteUserAsync(key, User.GetUserId(), cancellationToken);

            return StatusCode(StatusCodes.Status204NoContent);
        }
    }
}