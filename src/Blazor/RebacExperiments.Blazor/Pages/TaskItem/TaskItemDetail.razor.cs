// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Components;
using RebacExperiments.Shared.ApiSdk.Models;

namespace RebacExperiments.Blazor.Pages
{
    public partial class TaskItemDetail
    {
        /// <summary>
        /// The ID of the TaskItem to display.
        /// </summary>
        [Parameter]
        public int Id { get; set; }

        /// <summary>
        /// The TaskItem Details to be displayed.
        /// </summary>
        public TaskItem CurrentTaskItem { get; set; } = new TaskItem();

        protected override async Task OnInitializedAsync()
        {
            CurrentTaskItem = await GetTaskItemAsync(Id);

            await base.OnInitializedAsync();
        }

        private async Task<TaskItem> GetTaskItemAsync(int id)
        {
            var taskItem = await ApiClient.Odata.TaskItems[id].GetAsync();

            if (taskItem == null)
            {
                return new TaskItem();
            }

            return taskItem;
        }
    }
}
