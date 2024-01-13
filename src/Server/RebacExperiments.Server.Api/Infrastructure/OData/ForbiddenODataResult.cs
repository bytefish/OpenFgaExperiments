﻿// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.OData;

namespace RebacExperiments.Server.Api.Infrastructure.OData
{
    /// <summary>
    /// Represents a result that when executed will produce a Forbidden (403) response.
    /// </summary>
    /// <remarks>This result creates an <see cref="ODataError"/> with status code: 403.</remarks>
    public class ForbiddenODataResult : ForbidResult, IODataErrorResult
    {
        /// <summary>
        /// OData Error.
        /// </summary>
        public ODataError Error { get; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="odataError">OData Error.</param>
        public ForbiddenODataResult(ODataError odataError)
        {
            Error = odataError;
        }

        /// <inheritdoc/>
        public async override Task ExecuteResultAsync(ActionContext context)
        {
            ObjectResult objectResult = new ObjectResult(Error)
            {
                StatusCode = StatusCodes.Status403Forbidden
            };

            await objectResult.ExecuteResultAsync(context).ConfigureAwait(false);
        }
    }
}
