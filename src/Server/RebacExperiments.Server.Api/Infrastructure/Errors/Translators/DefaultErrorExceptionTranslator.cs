// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.OData;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using RebacExperiments.Server.Api.Infrastructure.OData;

namespace RebacExperiments.Server.Api.Infrastructure.Errors.Translators
{
    public class DefaultErrorExceptionTranslator : IODataExceptionTranslator
    {
        private readonly ILogger<DefaultErrorExceptionTranslator> _logger;

        public DefaultErrorExceptionTranslator(ILogger<DefaultErrorExceptionTranslator> logger)
        {
            _logger = logger;
        }

        public Type ExceptionType => typeof(Exception);

        public ObjectODataErrorResult GetODataErrorResult(Exception exception, bool includeExceptionDetails)
        {
            _logger.TraceMethodEntry();

            var error = new ODataError
            {
                ErrorCode = ErrorCodes.InternalServerError,
                Message = "An Internal Server Error occured"
            };

            // Create the Inner Error
            error.InnerError = new ODataInnerError();
            
            if (includeExceptionDetails)
            {
                error.InnerError.Message = exception.Message;
                error.InnerError.StackTrace = exception.StackTrace;
                error.InnerError.TypeName = exception.GetType().Name;
            }

            return new ObjectODataErrorResult
            {
                Error = error,
                HttpStatusCode = StatusCodes.Status500InternalServerError,
            };
        }
    }
}
