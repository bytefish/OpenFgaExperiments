// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RebacExperiments.Server.Api.Infrastructure.OData;

namespace RebacExperiments.Server.Api.Infrastructure.Errors
{
    /// <summary>
    /// A Translator to convert from an <see cref="Exception"/> to an <see cref="ObjectODataErrorResult"/>.
    /// </summary>
    public interface IODataExceptionTranslator
    {
        /// <summary>
        /// Translates a given <see cref="Exception"/> into an <see cref="ObjectODataErrorResult"/>.
        /// </summary>
        /// <param name="exception">Exception to translate</param>
        /// <param name="includeExceptionDetails">A flag, if exception details should be included</param>
        /// <returns>The <see cref="ObjectODataErrorResult"/> for the <see cref="Exception"/></returns>
        ObjectODataErrorResult GetODataErrorResult(Exception exception, bool includeExceptionDetails);

        /// <summary>
        /// Gets or sets the Exception Type.
        /// </summary>
        Type ExceptionType { get; }
    }
}
