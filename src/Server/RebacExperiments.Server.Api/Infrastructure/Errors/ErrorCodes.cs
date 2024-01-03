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
        public const string InternalServerError = "Error_System_000001";

        /// <summary>
        /// BadRequest.
        /// </summary>
        public const string BadRequest = "Error_System_000002";

        /// <summary>
        /// Validation Error.
        /// </summary>
        public const string ValidationFailed = "Error_Validation_000001";

        /// <summary>
        /// General Authentication Error.
        /// </summary>
        public const string AuthenticationFailed = "Error_Auth_000001";

        /// <summary>
        /// Entity has not been found.
        /// </summary>
        public const string EntityNotFound = "Error_Entity_000001";

        /// <summary>
        /// Access to Entity has been unauthorized.
        /// </summary>
        public const string EntityUnauthorized = "Error_Entity_000002";

        /// <summary>
        /// Entity has been modified concurrently.
        /// </summary>
        public const string EntityConcurrencyFailure = "Error_Entity_000003";
    }
}
