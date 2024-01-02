// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using RebacExperiments.Blazor.Infrastructure;
using RebacExperiments.Blazor.Localization;
using RebacExperiments.Shared.ApiSdk.Models;

namespace RebacExperiments.Blazor.Pages
{
    public partial class TaskItemEdit
    {
        /// <summary>
        /// The ID of the TaskItem to display.
        /// </summary>
        [Parameter]
        public int Id { get; set; }

        /// <summary>
        /// Localizer to use for translating messages.
        /// </summary>
        [Inject]
        public IStringLocalizer<SharedResource> Loc { get; set; } = null!;

        /// <summary>
        /// The TaskItem Details to be displayed.
        /// </summary>
        public TaskItem CurrentTaskItem { get; set; } = new TaskItem();

        /// <inheritdoc/>
        protected override async Task OnInitializedAsync()
        {
            CurrentTaskItem = await GetTaskItemAsync(Id);

            await base.OnInitializedAsync();
        }

        /// <summary>
        /// Gets the <see cref="TaskItem"/> by its Id.
        /// </summary>
        /// <param name="id">Id of the TaskItem </param>
        /// <returns>The <see cref="TaskItem"/> or a new one</returns>
        private async Task<TaskItem> GetTaskItemAsync(int id)
        {
            var taskItem = await ApiClient.Odata.TaskItems[id].GetAsync();

            if (taskItem == null)
            {
                return new TaskItem();
            }

            return taskItem;
        }

        /// <summary>
        /// Submits the Form.
        /// </summary>
        /// <returns>An awaitable <see cref="Task"/></returns>
        private Task HandleValidSubmitAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Validates a <see cref="TaskItem"/>.
        /// </summary>
        /// <param name="taskItem">TaskItem to validate</param>
        /// <returns>The list of validation errors for the EditContext model fields</returns>
        private IEnumerable<ValidationError> ValidateTaskItem(TaskItem taskItem)
        {
            if (string.IsNullOrWhiteSpace(taskItem.Title))
            {
                yield return new ValidationError 
                { 
                    PropertyName = nameof(taskItem.Title), 
                    ErrorMessage = Loc.GetString("Validation_IsRequired", nameof(taskItem.Title)) 
                };
            }
            
            if(string.IsNullOrWhiteSpace(taskItem.Description))
            {
                yield return new ValidationError
                {
                    PropertyName = nameof(taskItem.Description),
                    ErrorMessage = Loc.GetString("Validation_IsRequired", nameof(taskItem.Description))
                };
            }

            if (taskItem.TaskItemPriority == null)
            {
                yield return new ValidationError
                {
                    PropertyName = nameof(taskItem.TaskItemPriority),
                    ErrorMessage = Loc.GetString("Validation_IsRequired", nameof(taskItem.TaskItemPriority))
                };
            }
        }
    }
}
