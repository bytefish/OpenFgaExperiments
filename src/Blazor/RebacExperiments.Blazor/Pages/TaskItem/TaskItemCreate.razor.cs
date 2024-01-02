// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Components;
using RebacExperiments.Shared.ApiSdk.Models;

namespace RebacExperiments.Blazor.Pages
{
    public partial class TaskItemCreate
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

        private async Task<TaskItem> CreateTaskItemAsync(int id)
        {
            var taskItem = await ApiClient.Odata.TaskItems.PostAsync(CurrentTaskItem);

            if (taskItem == null)
            {
                return new TaskItem();
            }

            return taskItem;
        }

        private async Task PersistTaskItem(TaskItem taskItem)
        {
            
        }
    }
}
