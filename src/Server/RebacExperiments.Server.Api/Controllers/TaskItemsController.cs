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
    public class TaskItemsController : ODataController
    {
        private readonly ILogger<TaskItemsController> _logger;

        public TaskItemsController(ILogger<TaskItemsController> logger)
        {
            _logger = logger;
        }

        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public async Task<IActionResult> GetTaskItem([FromServices] ITaskItemService taskItemService, [FromODataUri(Name = "key")] int key, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                throw new InvalidModelStateException
                {
                    ModelStateDictionary = ModelState
                };
            }

            var taskItem = await taskItemService.GetTaskItemByIdAsync(key, User.GetUserId(), cancellationToken);

            return Ok(taskItem);

        }

        [HttpGet]
        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public async Task<IActionResult> GetTaskItems([FromServices] ITaskItemService taskItemService, ODataQueryOptions<TaskItem> queryOptions, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                throw new InvalidModelStateException
                {
                    ModelStateDictionary = ModelState
                };
            }

            var taskItems = await taskItemService.GetTaskItemsByUserIdAsync(User.GetUserId(), cancellationToken);

            return Ok(queryOptions.ApplyTo(taskItems.AsQueryable()));
        }

        [HttpPost]
        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public async Task<IActionResult> PostTaskItem([FromServices] ITaskItemService taskItemService, [FromBody] TaskItem taskItem, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                throw new InvalidModelStateException
                {
                    ModelStateDictionary = ModelState
                };
            }

            await taskItemService.CreateTaskItemAsync(taskItem, User.GetUserId(), cancellationToken);

            return Created(taskItem);
        }

        [HttpPut]
        [HttpPatch]
        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public async Task<IActionResult> PatchTaskItem([FromServices] ITaskItemService taskItemService, [FromODataUri] int key, [FromBody] Delta<TaskItem> delta, CancellationToken cancellationToken)
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
            var taskItem = await taskItemService.GetTaskItemByIdAsync(key, User.GetUserId(), cancellationToken);

            // Patch the Values to it:
            delta.Patch(taskItem);

            // Update the Values:
            await taskItemService.UpdateTaskItemAsync(taskItem, User.GetUserId(), cancellationToken);

            return Updated(taskItem);
        }

        [HttpDelete]
        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public async Task<IActionResult> DeleteTaskItem([FromServices] ITaskItemService taskItemService, [FromODataUri(Name = "key")] int key, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                throw new InvalidModelStateException
                {
                    ModelStateDictionary = ModelState
                };
            }

            await taskItemService.DeleteTaskItemAsync(key, User.GetUserId(), cancellationToken);

            return StatusCode(StatusCodes.Status204NoContent);
        }
    }
}