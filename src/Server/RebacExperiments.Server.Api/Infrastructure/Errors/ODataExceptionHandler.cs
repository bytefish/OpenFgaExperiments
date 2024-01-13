// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Extensions.Options;
using Microsoft.OData;
using RebacExperiments.Server.Api.Infrastructure.Exceptions;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using RebacExperiments.Server.Api.Infrastructure.OData;
using System.Net;

namespace RebacExperiments.Server.Api.Infrastructure.Errors
{
    /// <summary>
    /// Handles errors returned by the application.
    /// </summary>
    public class ODataExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<ODataExceptionHandler> _logger;

        private readonly ODataExceptionHandlerOptions _options;

        public ODataExceptionHandler(ILogger<ODataExceptionHandler> logger, IOptions<ODataExceptionHandlerOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var exceptionHandlerFeature = httpContext.Features.Get<IExceptionHandlerFeature>();

            if (exceptionHandlerFeature == null)
            {
                return false;
            }

            var endpoint = exceptionHandlerFeature.Endpoint;

            if (endpoint == null)
            {
                return false;
            }
            
            // Get the Action Result from the Exception
            var actionResult = HandleException(httpContext, exception);

            // Get the Action Descriptor, if we are running in MVC
            var actionDescriptor = endpoint.Metadata.GetMetadata<ActionDescriptor>();
            
            if (actionDescriptor == null)
            {
                actionDescriptor = new ActionDescriptor();
            }

            var routeData = httpContext.GetRouteData();

            if(routeData == null)
            {
                routeData = new RouteData();
            }

            var actionContext = new ActionContext
            {
                HttpContext = httpContext,
                ActionDescriptor = actionDescriptor,
                RouteData = routeData,
            };

            await actionResult
                .ExecuteResultAsync(actionContext)
                .ConfigureAwait(false);

            return true;
        }

        public ActionResult HandleException(HttpContext httpContext, Exception exception)
        {
            _logger.TraceMethodEntry();

            _logger.LogError(exception, "Call to '{RequestPath}' failed due to an Exception", httpContext.Request.Path);

            return exception switch
            {
                // Auth
                AuthenticationFailedException e => HandleAuthenticationException(httpContext, e),
                AuthorizationFailedException e => HandleAuthorizationFailed(httpContext, e),
                // Entities
                EntityConcurrencyException e => HandleEntityConcurrencyException(httpContext, e),
                EntityNotFoundException e => HandleEntityNotFoundException(httpContext, e),
                EntityUnauthorizedAccessException e => HandleEntityUnauthorizedException(httpContext, e),
                // Rate Limiting
                RateLimitException e => HandleRateLimitException(httpContext, e),
                // Global Handler
                Exception e => HandleSystemException(httpContext, e)
            };
        }

        public ActionResult HandleInvalidModelStateException(HttpContext httpContext, InvalidModelStateException e)
        {
            _logger.TraceMethodEntry();

            if(e.ModelStateDictionary.IsValid)
            {
                throw new InvalidOperationException("Could not create an error response from a valid ModelStateDictionary");
            }

            ODataError error = new ODataError()
            {
                ErrorCode = ErrorCodes.BadRequest,
                Message = "One or more validation errors occured",
                Details = GetODataErrorDetails(e.ModelStateDictionary),
            };

            // If we have something like a Deserialization issue, the ModelStateDictionary has
            // a lower-level Exception. We cannot do anything sensible with exceptions, so 
            // we add them to the InnerError.
            var firstException = GetFirstException(e.ModelStateDictionary);

            AddInnerError(httpContext, error, firstException);

            return new BadRequestODataResult(error);
        }

        private Exception? GetFirstException(ModelStateDictionary modelStateDictionary)
        {
            _logger.TraceMethodEntry();

            foreach (var modelStateEntry in modelStateDictionary)
            {
                foreach (var modelError in modelStateEntry.Value.Errors)
                {
                    if (modelError.Exception != null)
                    {
                        return modelError.Exception;
                    }
                }
            }

            return null;
        }

        private List<ODataErrorDetail> GetODataErrorDetails(ModelStateDictionary modelStateDictionary)
        {
            _logger.TraceMethodEntry();

            // Validation Details
            var result = new List<ODataErrorDetail>();

            foreach (var modelStateEntry in modelStateDictionary)
            {
                foreach (var modelError in modelStateEntry.Value.Errors)
                {
                    // We cannot make anything sensible for the caller here. We log it, but then go on 
                    // as if nothing has happened. Alternative is to populate a chain of ODataInnerError 
                    // or abuse the ODataErrorDetails...
                    if (modelError.Exception != null)
                    {
                        _logger.LogError(modelError.Exception, "Invalid ModelState due to an exception");

                        continue;
                    }

                    // A ModelStateDictionary has nothing like an "ErrorCode" and it's not 
                    // possible with existing infrastructure to get an "ErrorCode" here. So 
                    // we set a generic one.
                    var errorCode = ErrorCodes.ValidationFailed;

                    var odataErrorDetail = new ODataErrorDetail
                    {
                        ErrorCode = errorCode,
                        Message = modelError.ErrorMessage,
                        Target = modelStateEntry.Key,
                    };

                    result.Add(odataErrorDetail);
                }
            }

            return result;
        }

        private UnauthorizedODataResult HandleAuthenticationException(HttpContext httpContext, AuthenticationFailedException e)
        {
            _logger.TraceMethodEntry();

            var error = new ODataError
            {
                ErrorCode = ErrorCodes.AuthenticationFailed,
                Message = e.ErrorMessage,
            };

            AddInnerError(httpContext, error, e);

            return new UnauthorizedODataResult(error);
        }

        private ForbiddenODataResult HandleAuthorizationFailed(HttpContext httpContext, AuthorizationFailedException e)
        {
            _logger.TraceMethodEntry();

            var error = new ODataError
            {
                ErrorCode = ErrorCodes.AuthorizationFailed,
                Message = e.ErrorMessage,
            };

            AddInnerError(httpContext, error, e);

            return new ForbiddenODataResult(error);
        }

        private ConflictODataResult HandleEntityConcurrencyException(HttpContext httpContext, EntityConcurrencyException e)
        {
            _logger.TraceMethodEntry();

            var error = new ODataError
            {
                ErrorCode = e.ErrorCode,
                Message = e.ErrorMessage,
            };

            AddInnerError(httpContext, error, e);

            return new ConflictODataResult(error);
        }

        private NotFoundODataResult HandleEntityNotFoundException(HttpContext httpContext, EntityNotFoundException e)
        {
            _logger.TraceMethodEntry();

            var error = new ODataError
            {
                ErrorCode = e.ErrorCode,
                Message = e.ErrorMessage,
            };

            AddInnerError(httpContext, error, e);

            return new NotFoundODataResult(error);
        }

        private ForbiddenODataResult HandleEntityUnauthorizedException(HttpContext httpContext, EntityUnauthorizedAccessException e)
        {
            _logger.TraceMethodEntry();

            var error = new ODataError
            {
                ErrorCode = e.ErrorCode,
                Message = e.ErrorMessage,
            };

            AddInnerError(httpContext, error, e);

            return new ForbiddenODataResult(error);
        }
        
        private ObjectODataErrorResult HandleRateLimitException(HttpContext httpContext, RateLimitException e)
        {
            _logger.TraceMethodEntry();

            var error = new ODataError
            {
                ErrorCode = ErrorCodes.TooManyRequests,
                Message = "Too many requests. The Rate Limit for the user has been exceeded"
            };

            AddInnerError(httpContext, error, e);

            return new ObjectODataErrorResult
            {
                Error = error,
                HttpStatusCode = (int)HttpStatusCode.TooManyRequests,
            };
        }

        private ObjectODataErrorResult HandleSystemException(HttpContext httpContext, Exception e)
        {
            _logger.TraceMethodEntry();

            var error = new ODataError
            {
                ErrorCode = ErrorCodes.InternalServerError,
                Message = "An Internal Server Error occured"
            };

            AddInnerError(httpContext, error, e);

            return new ObjectODataErrorResult
            {
                Error = error,
                HttpStatusCode = (int)HttpStatusCode.TooManyRequests,
            };
        }

        private void AddInnerError(HttpContext httpContext, ODataError error, Exception? e)
        {
            _logger.TraceMethodEntry();

            error.InnerError = new ODataInnerError();

            error.InnerError.Properties["trace-id"] = new ODataPrimitiveValue(httpContext.TraceIdentifier);

            if (e != null && _options.IncludeExceptionDetails)
            {
                error.InnerError.Message = e.Message;
                error.InnerError.StackTrace = e.StackTrace;
                error.InnerError.TypeName = e.GetType().Name;
            }
        }
    }
}