// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Options;
using Microsoft.OData;
using RebacExperiments.Server.Api.Infrastructure.Exceptions;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using RebacExperiments.Server.Api.Infrastructure.OData;

namespace RebacExperiments.Server.Api.Infrastructure.Errors
{
    /// <summary>
    /// Handles errors returned by the application.
    /// </summary>
    public class ExceptionToODataErrorMapper
    {
        private readonly ILogger<ExceptionToODataErrorMapper> _logger;

        private readonly ExceptionToODataErrorMapperOptions _options;
        private readonly Dictionary<Type, IODataExceptionTranslator> _translators;

        public ExceptionToODataErrorMapper(ILogger<ExceptionToODataErrorMapper> logger, IOptions<ExceptionToODataErrorMapperOptions> options, IEnumerable<IODataExceptionTranslator> translators)
        {
            _logger = logger;
            _options = options.Value;
            _translators = translators.ToDictionary(x => x.ExceptionType, x => x);
        }

        public ObjectODataErrorResult CreateODataErrorResult(HttpContext httpContext, Exception exception)
        {
            _logger.TraceMethodEntry();

            _logger.LogError(exception, "Call to '{RequestPath}' failed due to an Exception", httpContext.Request.Path);

            // Get the best matching translator for the exception ...
            var translator = GetTranslator(exception);

            // ... translate it to the Result ...
            var error = translator.GetODataErrorResult(exception, _options.IncludeExceptionDetails);

            // ... add error metadata, such as a Trace ID, ...
            AddMetadata(httpContext, error);

            // ... and return it.
            return error;
        }

        private void AddMetadata(HttpContext httpContext, ObjectODataErrorResult result)
        {
            if(result.Error.InnerError == null)
            {
                result.Error.InnerError = new ODataInnerError();
            }

            result.Error.InnerError.Properties["trace-id"] = new ODataPrimitiveValue(httpContext.TraceIdentifier);
        }

        private IODataExceptionTranslator GetTranslator(Exception e)
        {
            if (e is ApplicationErrorException)
            {
                if (_translators.TryGetValue(e.GetType(), out var translator))
                {
                    return translator;
                }

                return _translators[typeof(ApplicationErrorException)];
            }

            return _translators[typeof(Exception)];
        }
    }
}