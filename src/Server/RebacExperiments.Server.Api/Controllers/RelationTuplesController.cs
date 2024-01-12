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
using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.EntityFrameworkCore;
using RebacExperiments.Server.Api.Infrastructure.Exceptions;

namespace RebacExperiments.Server.Api.Controllers
{
    public class RelationTuplesController : ODataController
    {
        private readonly ILogger<RelationTuplesController> _logger;

        public RelationTuplesController(ILogger<RelationTuplesController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetRelationTuples([FromServices] IAclService aclService, ODataQueryOptions<StoredRelationTuple> options, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                throw new InvalidModelStateException
                {
                    ModelStateDictionary = ModelState
                };
            }

            var queryable = aclService.GetAllRelationshipsQueryable();

            return Ok(options.ApplyTo(queryable));
        }

        [HttpPost]
        public async Task<IActionResult> PostRelationTuple([FromServices] IAclService aclService, [FromBody] StoredRelationTuple relationTuple, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                throw new InvalidModelStateException
                {
                    ModelStateDictionary = ModelState
                };
            }

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

        [HttpDelete]
        public async Task<IActionResult> DeleteRelationTuple([FromServices] IAclService aclService, [FromODataUri(Name = "key")] string key, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                throw new InvalidModelStateException
                {
                    ModelStateDictionary = ModelState
                };
            }

            var tuplesToDelete = await aclService
                .GetAllRelationshipsQueryable()
                .Where(x => x.Id == key)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            if (tuplesToDelete.Count == 0)
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

        [HttpGet("/odata/GetCurrentStoreId()")]
        public IActionResult GetCurrentStoreId([FromServices] IConfiguration configuration)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                throw new InvalidModelStateException
                {
                    ModelStateDictionary = ModelState
                };
            }

            var currentStoreId = configuration.GetValue<string>("OpenFga:StoreId");

            return Ok(currentStoreId);
        }

        [HttpGet]
        public IActionResult GetCurrentRelationTuples([FromServices] IConfiguration configuration, [FromServices] IAclService aclService, ODataQueryOptions<StoredRelationTuple> options, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                throw new InvalidModelStateException
                {
                    ModelStateDictionary = ModelState
                };
            }

            var currentStoreId = configuration.GetValue<string>("OpenFga:StoreId")!;

            var queryable = aclService
                .GetAllRelationshipsByStoreQueryable(currentStoreId);

            return Ok(options.ApplyTo(queryable));
        }
    }
}