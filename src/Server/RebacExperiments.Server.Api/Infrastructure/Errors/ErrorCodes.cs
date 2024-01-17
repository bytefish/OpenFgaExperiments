// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace RebacExperiments.Server.Api.Infrastructure.Errors
{
    /// <summary>
    /// Error Codes used in the Application.
    /// </summary>
    public static class ErrorCodes
    {
        /// <summary>
        /// Internal Server Error.
        /// </summary>
        public const string InternalServerError = "ApiError_System_000001";

        /// <summary>
        /// Validation Error.
        /// </summary>
        public const string ValidationFailed = "ApiError_Validation_000001";

        /// <summary>
        /// General Authentication Error.
        /// </summary>
        public const string AuthenticationFailed = "ApiError_Auth_000001";

        /// <summary>
        /// General Authorization Error.
        /// </summary>
        public const string AuthorizationFailed = "ApiError_Auth_000002";

        /// <summary>
        /// Unauthorized.
        /// </summary>
        public const string Unauthorized = "ApiError_Auth_000003";

        /// <summary>
        /// Too many requests have been issued.
        /// </summary>
        public const string TooManyRequests = "ApiError_RateLimit_000001";

        /// <summary>
        /// Entity has not been found.
        /// </summary>
        public const string EntityNotFound = "ApiError_Entity_000001";

        /// <summary>
        /// Access to Entity has been unauthorized.
        /// </summary>
        public const string EntityUnauthorized = "ApiError_Entity_000002";

        /// <summary>
        /// Entity has been modified concurrently.
        /// </summary>
        public const string EntityConcurrencyFailure = "ApiError_Entity_000003";

        /// <summary>
        /// Resource has not been found.
        /// </summary>
        public const string ResourceNotFound = "ApiError_Routing_000001";

        /// <summary>
        /// Method was not allowed.
        /// </summary>
        public const string MethodNotAllowed = "ApiError_Routing_000002";
    }
}
