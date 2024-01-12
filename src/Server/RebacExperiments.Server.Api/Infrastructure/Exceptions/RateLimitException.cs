using RebacExperiments.Server.Api.Infrastructure.Errors;

namespace RebacExperiments.Server.Api.Infrastructure.Exceptions
{
    public class RateLimitException : ApplicationErrorException
    {
        /// <summary>
        /// Gets or sets an ErrorCode.
        /// </summary>
        public override string ErrorCode => ErrorCodes.TooManyRequests;

        /// <summary>
        /// Gets or sets an ErrorMessage.
        /// </summary>
        public override string ErrorMessage => $"RateLimitExceeded";

        /// <summary>
        /// Creates a new <see cref="EntityNotFoundException"/>.
        /// </summary>
        /// <param name="message">Error Message</param>
        /// <param name="innerException">Reference to the Inner Exception</param>
        public RateLimitException(string? message = null, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}
