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
using RebacExperiments.Server.Api.Infrastructure.Errors;
using RebacExperiments.Server.Api.Infrastructure.Exceptions;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using RebacExperiments.Server.Api.Services;
using RebacExperiments.Server.Database.Models;

namespace RebacExperiments.Server.Api.Controllers
{
    public class TeamsController : ODataController
    {
        private readonly ILogger<TeamsController> _logger;

        private readonly ExceptionToODataErrorMapper _exceptionToODataErrorMapper;

        public TeamsController(ILogger<TeamsController> logger, ExceptionToODataErrorMapper exceptionToODataErrorMapper)
        {
            _logger = logger;
            _exceptionToODataErrorMapper = exceptionToODataErrorMapper;
        }

        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public async Task<IActionResult> GetTeam([FromServices] ITeamService teamService, [FromODataUri(Name = "key")] int key, CancellationToken cancellationToken)
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

                var team = await teamService.GetTeamByIdAsync(key, User.GetUserId(), cancellationToken);

                return Ok(team);
            }
            catch (Exception exception)
            {
                return _exceptionToODataErrorMapper.CreateODataErrorResult(HttpContext, exception);
            }
        }

        [HttpGet]
        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public async Task<IActionResult> GetTeams([FromServices] ITeamService teamService, ODataQueryOptions<TaskItem> queryOptions, CancellationToken cancellationToken)
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

                var teams = await teamService.GetTeamsByUserIdAsync(User.GetUserId(), cancellationToken);

                return Ok(queryOptions.ApplyTo(teams.AsQueryable()));
            }
            catch (Exception exception)
            {
                return _exceptionToODataErrorMapper.CreateODataErrorResult(HttpContext, exception);
            }
        }

        [HttpPost]
        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public async Task<IActionResult> PostTeam([FromServices] ITeamService teamService, [FromBody] Team team, CancellationToken cancellationToken)
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

                await teamService.CreateTeamAsync(team, User.GetUserId(), cancellationToken);

                return Created(team);
            }
            catch (Exception exception)
            {
                return _exceptionToODataErrorMapper.CreateODataErrorResult(HttpContext, exception);
            }
        }

        [HttpPut]
        [HttpPatch]
        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public async Task<IActionResult> PatchTeam([FromServices] ITeamService teamService, [FromODataUri] int key, [FromBody] Delta<Team> delta, CancellationToken cancellationToken)
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

                // Get the TaskItem with the current values:
                var team = await teamService.GetTeamByIdAsync(key, User.GetUserId(), cancellationToken);

                // Patch the Values to it:
                delta.Patch(team);

                // Update the Values:
                await teamService.UpdateTeamAsync(team, User.GetUserId(), cancellationToken);

                return Updated(team);
            }
            catch (Exception exception)
            {
                return _exceptionToODataErrorMapper.CreateODataErrorResult(HttpContext, exception);
            }
        }


        [HttpDelete]
        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public async Task<IActionResult> DeleteTeam([FromServices] ITeamService teamService, [FromODataUri(Name = "key")] int key, CancellationToken cancellationToken)
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

                await teamService.DeleteTeamAsync(key, User.GetUserId(), cancellationToken);

                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception exception)
            {
                return _exceptionToODataErrorMapper.CreateODataErrorResult(HttpContext, exception);
            }
        }
    }
}