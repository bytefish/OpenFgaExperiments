// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Components;

namespace RebacExperiments.Blazor.Pages
{
    public partial class ErrorDataGrid
    {
        public class ErrorListItem
        {
            public required string Code { get; set; }

            public required string Title { get; set; }
        }

        private static IQueryable<ErrorListItem> Errors = new List<ErrorListItem>()
        {
                new ErrorListItem { Code = "ClientError_UnexpectedError", Title = "Unexpected Error" },
                new ErrorListItem { Code = "ApiError_Auth_000001", Title = "Authentication Failure" },
                new ErrorListItem { Code = "ApiError_Auth_000002", Title = "Missing Permissions" },
                new ErrorListItem { Code = "ApiError_Auth_000003", Title = "Unauthorized Access" },
                new ErrorListItem { Code = "ApiError_Entity_000001", Title = "Entity Not Found" },
                new ErrorListItem { Code = "ApiError_Entity_000002", Title = "Entity Permissions Missing" },
                new ErrorListItem { Code = "ApiError_Entity_000003", Title = "Concurrency Failure" },
                new ErrorListItem { Code = "ApiError_RateLimit_000001", Title = "Too Many Requests" },
                new ErrorListItem { Code = "ApiError_Routing_000001", Title = "Resource Not Found" },
                new ErrorListItem { Code = "ApiError_Routing_000002", Title = "Method Not Allowed" },
                new ErrorListItem { Code = "ApiError_System_000001", Title = "Internal Server Error" },
        }
        .AsQueryable();

        private void ShowDetails(ErrorListItem errorListItem)
        {
            NavigationManager.NavigateTo($"/Help/Errors/{errorListItem.Code}");
        }
    }
}
