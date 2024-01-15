// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData;
using RebacExperiments.Server.Api.Infrastructure.Errors;
using RebacExperiments.Server.Api.Infrastructure.Logging;

namespace RebacExperiments.Server.Api.Controllers
{
    public class ErrorController : ControllerBase
    {
        private readonly ILogger<ErrorController> _logger;

        private readonly ODataErrorMapper _odataErrorMapper;

        public ErrorController(ILogger<ErrorController> logger, ODataErrorMapper exceptionToODataErrorMapper)
        {
            _logger = logger;
            _odataErrorMapper = exceptionToODataErrorMapper;
        }

        [Route("/error")]
        public IActionResult HandleError()
        {
            _logger.TraceMethodEntry();

            var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>()!;

            var error = _odataErrorMapper.CreateODataErrorResult(HttpContext, exceptionHandlerFeature.Error);

            return new ContentResult
            {
                Content = error.ToString(),
                ContentType = "application/json",
                StatusCode = StatusCodes.Status400BadRequest
            };
        }

        [Route("/error/401")]
        public IActionResult HandleHttpStatus401()
        {
            _logger.TraceMethodEntry();

            var error = new ODataError
            {
                ErrorCode = ErrorCodes.Unauthorized,
                Message = "Unauthorized"
            };

            error.InnerError = new ODataInnerError();
            error.InnerError.Properties["trace-id"] = new ODataPrimitiveValue(HttpContext.TraceIdentifier);

     
            return new ContentResult
            {
                Content = error.ToString(),
                ContentType = "application/json",
                StatusCode = StatusCodes.Status401Unauthorized
            };
        }

        [Route("/error/404")]
        public IActionResult HandleHttpStatus404()
        {
            _logger.TraceMethodEntry();

            var error = new ODataError
            {
                ErrorCode = ErrorCodes.ResourceNotFound,
                Message = "ResourceNotFound"
            };
            
            error.InnerError = new ODataInnerError();
            error.InnerError.Properties["trace-id"] = new ODataPrimitiveValue(HttpContext.TraceIdentifier);

            return new ContentResult 
            { 
                Content = error.ToString(), 
                ContentType = "application/json", 
                StatusCode = StatusCodes.Status404NotFound 
            };
        }

        [Route("/error/405")]
        public IActionResult HandleHttpStatus405()
        {
            _logger.TraceMethodEntry();

            var error = new ODataError
            {
                ErrorCode = ErrorCodes.MethodNotAllowed,
                Message = "MethodNotAllowed"
            };

            error.InnerError = new ODataInnerError();
            error.InnerError.Properties["trace-id"] = new ODataPrimitiveValue(HttpContext.TraceIdentifier);

            return new ContentResult 
            { 
                Content = error.ToString(), 
                ContentType = "application/json",
                StatusCode = StatusCodes.Status405MethodNotAllowed
            };
        }

        [Route("/error/429")]
        public IActionResult HandleHttpStatus429()
        {
            _logger.TraceMethodEntry();

            var error = new ODataError
            {
                ErrorCode = ErrorCodes.TooManyRequests,
                Message = "TooManyRequests"
            };

            error.InnerError = new ODataInnerError();
            error.InnerError.Properties["trace-id"] = new ODataPrimitiveValue(HttpContext.TraceIdentifier);

            return new ContentResult
            {
                Content = error.ToString(),
                ContentType = "application/json",
                StatusCode = StatusCodes.Status429TooManyRequests
            };
        }
    }
}