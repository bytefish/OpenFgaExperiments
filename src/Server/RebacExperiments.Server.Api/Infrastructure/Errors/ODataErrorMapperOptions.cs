// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace RebacExperiments.Server.Api.Infrastructure.Errors
{
    /// <summary>
    /// Options for the <see cref="ODataErrorMapper"/>.
    /// </summary>
    public class ODataErrorMapperOptions
    {
        /// <summary>
        /// Gets or sets the option to include the Exception Details in the response.
        /// </summary>
        public bool IncludeExceptionDetails { get; set; } = false;
    }
}