// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RebacExperiments.Server.Api.Infrastructure.Errors;

namespace RebacExperiments.Server.Api.Infrastructure.Exceptions
{
    public class EntityConcurrencyException : ApplicationErrorException
    {
        /// <inheritdoc/>
        public override string ErrorCode => ErrorCodes.EntityConcurrencyFailure;

        /// <inheritdoc/>
        public override string ErrorMessage => $"EntityConcurrencyFailure (Entity = {EntityName}, EntityID = {EntityId})";

        /// <inheritdoc/>
        public override int HttpStatusCode => StatusCodes.Status409Conflict;

        /// <summary>
        /// Gets or sets the Entity Name.
        /// </summary>
        public required string EntityName { get; set; }

        /// <summary>
        /// Gets or sets the EntityId.
        /// </summary>
        public required int EntityId { get; set; }

        /// <summary>
        /// Creates a new <see cref="EntityConcurrencyException"/>.
        /// </summary>
        /// <param name="message">Error Message</param>
        /// <param name="innerException">Reference to the Inner Exception</param>
        public EntityConcurrencyException(string? message = null, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}
