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
using RebacExperiments.Server.Api.Infrastructure.Database;
using RebacExperiments.Server.Api.Infrastructure.Errors;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using RebacExperiments.Server.Api.Services;
using RebacExperiments.Server.Api.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using RebacExperiments.Server.Api.Infrastructure.Authorization;

namespace RebacExperiments.Server.Api.Controllers
{
    public class AclController : ODataController
    {
        private readonly ILogger<AclController> _logger;
        private readonly ApplicationErrorHandler _applicationErrorHandler;

        public AclController(ILogger<AclController> logger, ApplicationErrorHandler applicationErrorHandler)
        {
            _logger = logger;
            _applicationErrorHandler = applicationErrorHandler;
        }

        [HttpPost("odata/CreateRelationTuple")]
        public async Task<IActionResult> CreateRelationTuple([FromServices] IAclService aclService, [FromBody] RelationTuple relationTuple, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                return _applicationErrorHandler.HandleInvalidModelState(HttpContext, ModelState);
            }

            try
            {

                await aclService
                    .AddRelationshipsAsync(new[] { relationTuple }, cancellationToken)
                    .ConfigureAwait(false);

                return Ok();
            }
            catch (Exception ex)
            {
                return _applicationErrorHandler.HandleException(HttpContext, ex);
            }
        }

        [HttpPost("odata/DeleteRelationTuple")]
        public async Task<IActionResult> DeleteRelationTuple([FromServices] IAclService aclService, [FromBody] RelationTuple relationTuple, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                return _applicationErrorHandler.HandleInvalidModelState(HttpContext, ModelState);
            }

            try
            {

                await aclService
                    .DeleteRelationshipsAsync(new[] { relationTuple }, cancellationToken)
                    .ConfigureAwait(false);

                return Ok();
            }
            catch (Exception ex)
            {
                return _applicationErrorHandler.HandleException(HttpContext, ex);
            }
        }
    }
}