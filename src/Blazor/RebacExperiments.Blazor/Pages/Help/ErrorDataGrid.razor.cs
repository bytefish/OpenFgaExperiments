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

            public required string Message { get; set; }
        }

        private IQueryable<ErrorListItem> Errors = new List<ErrorListItem>().AsQueryable();

        protected override void OnInitialized()
        {
            Errors = new List<ErrorListItem>()
            {
                    new ErrorListItem { Code = "ClientError_UnexpectedError", Title = "Unexpected Error", Message = Loc["ClientError_UnexpectedError"] },
                    new ErrorListItem { Code = "ApiError_Auth_000001", Title = "Authentication Failure", Message = Loc["ApiError_Auth_000001"] },
                    new ErrorListItem { Code = "ApiError_Auth_000002", Title = "Missing Permissions", Message = Loc["ApiError_Auth_000002"]},
                    new ErrorListItem { Code = "ApiError_Auth_000003", Title = "Unauthorized Access", Message = Loc["ApiError_Auth_000003"] },
                    new ErrorListItem { Code = "ApiError_Entity_000001", Title = "Entity Not Found", Message = Loc["ApiError_Entity_000001"] },
                    new ErrorListItem { Code = "ApiError_Entity_000002", Title = "Entity Permissions Missing", Message = Loc["ApiError_Entity_000002"] },
                    new ErrorListItem { Code = "ApiError_Entity_000003", Title = "Concurrency Failure", Message = Loc["ApiError_Entity_000003"] },
                    new ErrorListItem { Code = "ApiError_RateLimit_000001", Title = "Too Many Requests", Message = Loc["ApiError_RateLimit_000001"] },
                    new ErrorListItem { Code = "ApiError_Routing_000001", Title = "Resource Not Found", Message = Loc["ApiError_Routing_000001"] },
                    new ErrorListItem { Code = "ApiError_Routing_000002", Title = "Method Not Allowed", Message = Loc["ApiError_Routing_000002"] },
                    new ErrorListItem { Code = "ApiError_System_000001", Title = "Internal Server Error", Message = Loc["ApiError_System_000001"] },
            }
            .AsQueryable();
        }

        private void ShowDetails(ErrorListItem errorListItem)
        {
            NavigationManager.NavigateTo($"/Help/Errors/{errorListItem.Code}");
        }
    }
}