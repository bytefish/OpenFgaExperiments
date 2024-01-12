// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc.ModelBinding;
using RebacExperiments.Server.Api.Infrastructure.Errors;

namespace RebacExperiments.Server.Api.Infrastructure.Exceptions
{
    public class InvalidModelStateException : ApplicationErrorException
    {
        /// <summary>
        /// Gets or sets an error code.
        /// </summary>
        public override string ErrorCode => ErrorCodes.ValidationFailed;

        /// <summary>
        /// Gets or sets an error code.
        /// </summary>
        public override string ErrorMessage => $"ValidationFailure";

        /// <summary>
        /// Gets or sets the ModelStateDictionary.
        /// </summary>
        public required ModelStateDictionary ModelStateDictionary { get; set; }


        /// <summary>
        /// Creates a new <see cref="EntityNotFoundException"/>.
        /// </summary>
        /// <param name="message">Error Message</param>
        /// <param name="innerException">Reference to the Inner Exception</param>
        public InvalidModelStateException(string? message = null, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}
