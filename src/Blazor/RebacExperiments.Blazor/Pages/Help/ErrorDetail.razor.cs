// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Components;

namespace RebacExperiments.Blazor.Pages
{
    public partial class ErrorDetail
    {
        /// <summary>
        /// Gets or sets the Error Code to view information for.
        /// </summary>
        [Parameter]
        public string? ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets an Exception useful to report errors.
        /// </summary>
        [Parameter]
        public Exception? Exception { get; set; }

        private string? _assetUrl;

        protected override void OnParametersSet()
        {
            if(ErrorCode != null)
            {
                _assetUrl = $"/docs/errors/{ErrorCode}.md";
            }            
        }
    }
}
