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
    public class OrganizationsController : ODataController
    {
        private readonly ILogger<OrganizationsController> _logger;

        public OrganizationsController(ILogger<OrganizationsController> logger)
        {
            _logger = logger;
        }

        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public async Task<IActionResult> GetOrganization([FromServices] IOrganizationService organizationService, [FromODataUri(Name = "key")] int key, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                throw new InvalidModelStateException
                {
                    ModelStateDictionary = ModelState
                };
            }

            var organization = await organizationService.GetOrganizationByIdAsync(key, User.GetUserId(), cancellationToken);

            return Ok(organization);
        }

        [HttpGet]
        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public async Task<IActionResult> GetOrganizations([FromServices] IOrganizationService organizationService, ODataQueryOptions<TaskItem> queryOptions, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                throw new InvalidModelStateException
                {
                    ModelStateDictionary = ModelState
                };
            }

            var organizations = await organizationService.GetOrganizationsByUserIdAsync(User.GetUserId(), cancellationToken);

            return Ok(queryOptions.ApplyTo(organizations.AsQueryable()));
        }

        [HttpPost]
        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public async Task<IActionResult> PostOrganization([FromServices] IOrganizationService organizationService, [FromBody] Organization Organization, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                throw new InvalidModelStateException
                {
                    ModelStateDictionary = ModelState
                };
            }

            await organizationService.CreateOrganizationAsync(Organization, User.GetUserId(), cancellationToken);

            return Created(Organization);
        }

        [HttpPut]
        [HttpPatch]
        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public async Task<IActionResult> PatchOrganization([FromServices] IOrganizationService organizationService, [FromODataUri] int key, [FromBody] Delta<Organization> delta, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                throw new InvalidModelStateException
                {
                    ModelStateDictionary = ModelState
                };
            }

            // Get the TaskItem with the current values:
            var organization = await organizationService.GetOrganizationByIdAsync(key, User.GetUserId(), cancellationToken);

            // Patch the Values to it:
            delta.Patch(organization);

            // Update the Values:
            await organizationService.UpdateOrganizationAsync(organization, User.GetUserId(), cancellationToken);

            return Updated(organization);
        }

        [HttpDelete]
        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public async Task<IActionResult> DeleteOrganization([FromServices] IOrganizationService organizationService, [FromODataUri(Name = "key")] int key, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                throw new InvalidModelStateException
                {
                    ModelStateDictionary = ModelState
                };
            }

            await organizationService.DeleteOrganizationAsync(key, User.GetUserId(), cancellationToken);

            return StatusCode(StatusCodes.Status204NoContent);
        }
    }
}