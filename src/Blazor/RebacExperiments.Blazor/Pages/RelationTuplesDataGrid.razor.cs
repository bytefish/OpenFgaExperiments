// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RebacExperiments.Blazor.Components;
using Microsoft.AspNetCore.Components;
using RebacExperiments.Blazor.Infrastructure;
using RebacExperiments.Shared.ApiSdk.Models;
using RebacExperiments.Blazor.Extensions;
using RebacExperiments.Blazor.Shared.OData;
using Microsoft.FluentUI.AspNetCore.Components;

namespace RebacExperiments.Blazor.Pages
{
    public partial class RelationTuplesDataGrid
    {
        /// <summary>
        /// Provides the Data Items.
        /// </summary>
        private GridItemsProvider<RelationTuple> RelationTuplesProvider = default!;

        /// <summary>
        /// DataGrid.
        /// </summary>
        private FluentDataGrid<RelationTuple> DataGrid = default!;

        /// <summary>
        /// The current Pagination State.
        /// </summary>
        private readonly PaginationState Pagination = new() { ItemsPerPage = 10 };

        /// <summary>
        /// The current Filter State.
        /// </summary>
        private readonly FilterState FilterState = new();

        /// <summary>
        /// Reacts on Paginator Changes.
        /// </summary>
        private readonly EventCallbackSubscriber<FilterState> CurrentFiltersChanged;

        public RelationTuplesDataGrid()
        {
            CurrentFiltersChanged = new(EventCallback.Factory.Create<FilterState>(this, RefreshData));
        }

        protected override Task OnInitializedAsync()
        {
            RelationTuplesProvider = async request =>
            {
                var response = await GetRelationTuples(request);

                if(response == null)
                {
                    return GridItemsProviderResult.From(items: new List<RelationTuple>(), totalItemCount: 0);
                }

                var entities = response.Value;

                if (entities == null)
                {
                    return GridItemsProviderResult.From(items: new List<RelationTuple>(), totalItemCount: 0);
                }

                int count = response.GetODataCount();

                return GridItemsProviderResult.From(items: entities, totalItemCount: count);
            };
            
            return base.OnInitializedAsync();
        }


        /// <inheritdoc />
        protected override Task OnParametersSetAsync()
        {
            // The associated filter state may have been added/removed/replaced
            CurrentFiltersChanged.SubscribeOrMove(FilterState.CurrentFiltersChanged);

            return Task.CompletedTask;
        }

        private Task RefreshData()
        {
            return DataGrid.RefreshDataAsync();
        }

        private async Task<RelationTupleCollectionResponse?> GetRelationTuples(GridItemsProviderRequest<RelationTuple> request)
        {
            // Extract all Sort Columns from the Blazor FluentUI DataGrid
            var sortColumns = DataGridUtils.GetSortColumns(request);

            // Extract all Filters from the Blazor FluentUI DataGrid
            var filters = FilterState.Filters.Values.ToList();

            // Build the ODataQueryParameters using the ODataQueryParametersBuilder
            var parameters = ODataQueryParameters.Builder
                .SetPage(Pagination.CurrentPageIndex + 1, Pagination.ItemsPerPage)
                .SetFilter(filters)
                .AddOrderBy(sortColumns)
                .Build();

            // Get the Data using the ApiClient from the SDK
            return await ApiClient.Odata.RelationTuples.GetAsync(request =>
            {
                request.QueryParameters.Count = true;
                request.QueryParameters.Top = parameters.Top;
                request.QueryParameters.Skip = parameters.Skip;

                if (!string.IsNullOrWhiteSpace(parameters.Filter))
                {
                    request.QueryParameters.Filter = parameters.Filter;
                }

                if (parameters.OrderBy != null)
                {
                    request.QueryParameters.Orderby = parameters.OrderBy;
                }
            });    
        }
    }
}
