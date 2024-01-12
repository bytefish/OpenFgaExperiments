using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.OData;

namespace RebacExperiments.Server.Api.Infrastructure.OData
{
    /// <summary>
    /// Represents a result that when executed will produce an <see cref="ActionResult"/>.
    /// </summary>
    /// <remarks>This result creates an <see cref="ODataError"/> response.</remarks>
    public class ObjectODataErrorResult : ActionResult, IODataErrorResult
    {
        /// <summary>
        /// OData error.
        /// </summary>
        public required ODataError Error { get; set; }

        /// <summary>
        /// Http Status Code.
        /// </summary>
        public required int HttpStatusCode { get; set; }

        /// <inheritdoc/>
        public async override Task ExecuteResultAsync(ActionContext context)
        {
            ObjectResult objectResult = new ObjectResult(Error)
            {
                StatusCode = HttpStatusCode
            };

            await objectResult.ExecuteResultAsync(context).ConfigureAwait(false);
        }
    }
}
