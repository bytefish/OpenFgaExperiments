// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using RebacExperiments.Blazor.Localization;
using RebacExperiments.Shared.ApiSdk.Models;

namespace RebacExperiments.Blazor.Components
{
    public partial class TaskItemPrioritySelector
    {
        /// <summary>
        /// Localizer.
        /// </summary>
        [Inject]
        public IStringLocalizer<SharedResource> Loc { get; set; } = default!;

        /// <summary>
        /// Text used on aria-label attribute.
        /// </summary>
        [Parameter]
        public virtual string? Title { get; set; }

        /// <summary>
        /// If true, will disable the list of items.
        /// </summary>
        [Parameter]
        public virtual bool Disabled { get; set; } = false;

        /// <summary>
        /// If true, will make the selection required.
        /// </summary>
        [Parameter]
        public virtual bool Required { get; set; } = false;

        /// <summary>
        /// Gets or sets the content to be rendered inside the component.
        /// In this case list of FluentOptions
        /// </summary>
        [Parameter]
        public virtual RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// All selectable Sort Options.
        /// </summary>
        [Parameter]
        public TaskItemPriorityEnum[] TaskItemPriorities { get; set; } = new TaskItemPriorityEnum[]
        {
            TaskItemPriorityEnum.Low,
            TaskItemPriorityEnum.Normal,
            TaskItemPriorityEnum.High,
        };

        /// <summary>
        /// The TaskItem Priority.
        /// </summary>
        [Parameter]
        public TaskItemPriorityEnum TaskItemPriority { get; set; }

        /// <summary>
        /// Invoked, when the TaskItemPriority has changed.
        /// </summary>
        [Parameter]
        public EventCallback<TaskItemPriorityEnum> TaskItemPriorityChanged { get; set; }

        /// <summary>
        /// Value.
        /// </summary>
        string? _value { get; set; }

        /// <summary>
        /// Filter Operator.
        /// </summary>
        private TaskItemPriorityEnum _taskItemPriority { get; set; }

        protected override void OnParametersSet()
        {
            _taskItemPriority = TaskItemPriority;
            _value = TaskItemPriority.ToString();
        }

        public void OnSelectedValueChanged(TaskItemPriorityEnum value)
        {
            _taskItemPriority = value;
            _value = value.ToString();

            TaskItemPriorityChanged.InvokeAsync(_taskItemPriority);
        }
    }
}