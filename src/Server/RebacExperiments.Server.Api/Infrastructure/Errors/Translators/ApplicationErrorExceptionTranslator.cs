// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.OData.Formatter.Serialization;
using Microsoft.OData;
using RebacExperiments.Server.Api.Infrastructure.Exceptions;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using RebacExperiments.Server.Api.Infrastructure.OData;
using System.IdentityModel.Tokens.Jwt;

namespace RebacExperiments.Server.Api.Infrastructure.Errors.Translators
{
    public class ApplicationErrorExceptionTranslator : IODataExceptionTranslator
    {
        private readonly ILogger<ApplicationErrorExceptionTranslator> _logger;

        public ApplicationErrorExceptionTranslator(ILogger<ApplicationErrorExceptionTranslator> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public ObjectODataErrorResult GetODataErrorResult(Exception exception, bool includeExceptionDetails)
        {
            _logger.TraceMethodEntry();

            var applicationErrorException = (ApplicationErrorException)exception;

            return InternalGetODataErrorResult(applicationErrorException, includeExceptionDetails);
        }

        private ObjectODataErrorResult InternalGetODataErrorResult(ApplicationErrorException exception, bool includeExceptionDetails)
        {
            var error = new ODataError
            {
                ErrorCode = exception.ErrorCode,
                Message = exception.ErrorMessage,
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
                HttpStatusCode = exception.HttpStatusCode,
            };
        }

        /// <inheritdoc/>
        public Type ExceptionType => typeof(ApplicationErrorException);
    }
}
