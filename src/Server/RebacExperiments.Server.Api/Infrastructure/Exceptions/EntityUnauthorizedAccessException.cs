﻿// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RebacExperiments.Server.Api.Infrastructure.Errors;

namespace RebacExperiments.Server.Api.Infrastructure.Exceptions
{
    public class EntityUnauthorizedAccessException : ApplicationErrorException
    {
        /// <inheritdoc/>
        public override string ErrorCode => ErrorCodes.EntityUnauthorized;

        /// <inheritdoc/>        
        public override string ErrorMessage => $"EntityUnauthorizedAccess (User = {UserId}, Entity = {EntityName}, EntityID = {EntityId})";

        /// <inheritdoc/>
        public override int HttpStatusCode => StatusCodes.Status403Forbidden;

        /// <summary>
        /// Gets or sets the User ID.
        /// </summary>
        public required int UserId { get; set; }

        /// <summary>
        /// Gets or sets the Entity Name.
        /// </summary>
        public required string EntityName { get; set; }

        /// <summary>
        /// Gets or sets the EntityId.
        /// </summary>
        public required int EntityId { get; set; }


        /// <summary>
        /// Creates a new <see cref="EntityUnauthorizedAccessException"/>.
        /// </summary>
        /// <param name="message">Error Message</param>
        /// <param name="innerException">Reference to the Inner Exception</param>
        public EntityUnauthorizedAccessException(string? message = null, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}
