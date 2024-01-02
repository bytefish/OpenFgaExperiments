// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Components;
using RebacExperiments.Shared.ApiSdk.Models;
using System.ComponentModel.DataAnnotations;

namespace RebacExperiments.Blazor.Components
{
    public class TaskItemEditValidatorAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var taskItem = validationContext.GetRequiredService<TaskItem>();

            return ValidationResult.Success;
        }
    }

    public partial class TaskItemEditForm
    {
        /// <summary>
        /// The TaskItem being edited.
        /// </summary>
        [Parameter]
        [TaskItemEditValidator]
        public TaskItem TaskItem { get; set; } = null!;

        /// <summary>
        /// The Callback to be invoked, when persisting the TaskItem.
        /// </summary>
        [Parameter]
        public EventCallback<TaskItem> PersistTaskItem { get; set; }

        /// <summary>
        /// Signals, if the TaskItem is being processed.
        /// </summary>
        public bool Processing { get; set; }

        protected async Task SaveTaskItemAsync()
        {
            try
            {
                Processing = true;

                await PersistTaskItem.InvokeAsync(TaskItem);
            }
            finally
            {
                Processing = false;
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Processing = false;

            if (TaskItem == null)
            {
                TaskItem = new TaskItem();
            }
        }

        public async Task HandleValidSubmitAsync()
        {
            await SaveTaskItemAsync();
        }
    }
}