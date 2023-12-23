// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using RebacExperiments.Server.Api.Infrastructure.Errors;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using RebacExperiments.Server.Api.Services;
using RebacExperiments.Server.Api.Infrastructure.Authorization;
using Microsoft.AspNetCore.OData.Query;
using System.Threading;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace RebacExperiments.Server.Api.Controllers
{
    public class RelationTuplesController : ODataController
    {
        private readonly ILogger<RelationTuplesController> _logger;
        private readonly ApplicationErrorHandler _applicationErrorHandler;

        public RelationTuplesController(ILogger<RelationTuplesController> logger, ApplicationErrorHandler applicationErrorHandler)
        {
            _logger = logger;
            _applicationErrorHandler = applicationErrorHandler;
        }

        [HttpGet]
        public async Task<IActionResult> GetRelationTuples([FromServices] IAclService aclService, ODataQueryOptions<StoredRelationTuple> query, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                return _applicationErrorHandler.HandleInvalidModelState(HttpContext, ModelState);
            }

            try
            {
                var result = await aclService
                    .GetAllRelationshipsAsync(tuples => (IQueryable<StoredRelationTuple>) query.ApplyTo(tuples), cancellationToken)
                    .ConfigureAwait(false);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return _applicationErrorHandler.HandleException(HttpContext, ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostRelationTuple([FromServices] IAclService aclService, [FromBody] StoredRelationTuple relationTuple, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                return _applicationErrorHandler.HandleInvalidModelState(HttpContext, ModelState);
            }

            try
            {
                var tupleToInsert = new RelationTuple
                {
                    Object = relationTuple.Object,
                    Relation = relationTuple.Relation,
                    Subject = relationTuple.Subject
                };

                await aclService
                    .AddRelationshipsAsync(new[] { tupleToInsert }, cancellationToken)
                    .ConfigureAwait(false);

                return Ok();
            }
            catch (Exception ex)
            {
                return _applicationErrorHandler.HandleException(HttpContext, ex);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRelationTuple([FromServices] IAclService aclService, [FromODataUri(Name = "key")] string key, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                return _applicationErrorHandler.HandleInvalidModelState(HttpContext, ModelState);
            }

            try
            {
                var tuplesToDelete = await aclService
                    .GetAllRelationshipsAsync(q => q.Where(x => x.Id == key), cancellationToken)
                    .ConfigureAwait(false);

                if(tuplesToDelete.Count == 0)
                {
                    return NotFound();
                }

                var tupleToDelete = new RelationTuple
                {
                    Object = tuplesToDelete[0].Object,
                    Relation = tuplesToDelete[0].Relation,
                    Subject = tuplesToDelete[0].Subject
                };

                await aclService
                    .DeleteRelationshipsAsync(new[] { tupleToDelete }, cancellationToken)
                    .ConfigureAwait(false);

                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                return _applicationErrorHandler.HandleException(HttpContext, ex);
            }
        }

        [HttpGet("/odata/GetCurrentStoreId()")]
        public IActionResult GetCurrentStoreId([FromServices] IConfiguration configuration)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                return _applicationErrorHandler.HandleInvalidModelState(HttpContext, ModelState);
            }

            try
            {
                var currentStoreId = configuration.GetValue<string>("OpenFga:StoreId");

                return Ok(currentStoreId);
            }
            catch (Exception ex)
            {
                return _applicationErrorHandler.HandleException(HttpContext, ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentRelationTuples([FromServices] IConfiguration configuration, [FromServices] IAclService aclService, ODataQueryOptions<StoredRelationTuple> query, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                return _applicationErrorHandler.HandleInvalidModelState(HttpContext, ModelState);
            }

            try
            {
                var currentStoreId = configuration.GetValue<string>("OpenFga:StoreId")!;
                
                var tuples = await aclService
                    .GetAllRelationshipsByStoreAsync(currentStoreId, tuples => (IQueryable<StoredRelationTuple>)query.ApplyTo(tuples), cancellationToken)
                    .ConfigureAwait(false);

                return Ok(tuples);
            }
            catch (Exception ex)
            {
                return _applicationErrorHandler.HandleException(HttpContext, ex);
            }


        }
    }
}