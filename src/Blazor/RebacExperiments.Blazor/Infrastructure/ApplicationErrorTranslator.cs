using Microsoft.Extensions.Localization;
using RebacExperiments.Blazor.Localization;
using RebacExperiments.Shared.ApiSdk.Models.ODataErrors;
using System.Diagnostics.CodeAnalysis;

namespace RebacExperiments.Blazor.Infrastructure
{
    public class ApplicationErrorTranslator
    {
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;

        public ApplicationErrorTranslator(IStringLocalizer<SharedResource> sharedLocalizer) 
        { 
            _sharedLocalizer = sharedLocalizer;
        }

        public string GetErrorMessage(Exception exception) 
        {
            return exception switch
            {
                ODataError e => GetErrorMessageFromODataError(e),
                Exception e => GetErrorMessageFromException(e),
            };
        }

        private string GetErrorMessageFromODataError(ODataError error)
        {
            // Extract the ErrorCode from the OData MainError.
            string errorCode = error.Error!.Code!;

            // And get the Error Message by the Error Code.
            string errorCodeMessage = _sharedLocalizer[errorCode];

            // Format with Trace ID for correlating user error reports with logs.
            if(TryGetRequestTraceId(error.Error!, out string? traceId))
            {
                return $"{errorCodeMessage} (TraceID = {traceId})";
            }

            return errorCodeMessage;
        }

        private string GetErrorMessageFromException(Exception e)
        {
            string errorMessage = _sharedLocalizer["ApplicationError_Exception"];

            return errorMessage;
        }

        private bool TryGetRequestTraceId(MainError mainError, [NotNullWhen(true)] out string? requestTraceId)
        {
            requestTraceId = null;

            if(mainError.Innererror == null)
            {
                return false;
            }

            var innerError = mainError.Innererror;

            if(!innerError.AdditionalData.ContainsKey("trace-id"))
            {
                return false;
            }

            requestTraceId = innerError.AdditionalData["trace-id"] as string;

            if(requestTraceId == null)
            {
                return false;
            }

            return true;
        }
    }
}
