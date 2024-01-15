// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.OData;
using Microsoft.OpenApi;
using RebacExperiments.Server.Api.Infrastructure.Errors;
using RebacExperiments.Server.Api.Infrastructure.OData;
using Microsoft.OpenApi.Extensions;
using Microsoft.OData.Edm;
using RebacExperiments.Server.Api.Infrastructure.Logging;

namespace RebacExperiments.Server.Api.Controllers
{
    /// <summary>
    /// This Controller exposes an Endpoint for the OpenAPI Schema, which will be generated from an <see cref="IEdmModel"/>.
    /// </summary>
    public class OpenApiController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;

        private readonly ODataErrorMapper _odataErrorMapper;

        public OpenApiController(ILogger<AuthenticationController> logger, ODataErrorMapper odataErrorMapper)
        {
            _logger = logger;
            _odataErrorMapper = odataErrorMapper;
        }

        [HttpGet("odata/openapi.json")]
        public IActionResult GetOpenApiJson()
        {
            _logger.TraceMethodEntry();

            try
            {
                var edmModel = ApplicationEdmModel.GetEdmModel();

                var openApiSettings = new OpenApiConvertSettings
                {
                    ServiceRoot = new("https://localhost:5000"),
                    PathPrefix = "odata",
                    EnableKeyAsSegment = true,
                };

                var openApiDocument = edmModel.ConvertToOpenApi(openApiSettings);

                var openApiDocumentAsJson = openApiDocument.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

                return Content(openApiDocumentAsJson, "application/json");
            }
            catch (Exception exception)
            {
                return _odataErrorMapper.CreateODataErrorResult(HttpContext, exception);
            }
        }
    }
}