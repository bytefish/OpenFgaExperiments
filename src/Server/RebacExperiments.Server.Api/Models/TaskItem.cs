// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace RebacExperiments.Server.Api.Models
{
    /// <summary>
    /// A Task within the system, which may be assigned to Organizations, Teams, Users, ...
    /// </summary>
    public class TaskItem : Entity
    {
        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        public required string Title { get; set; }

        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        public required string Description { get; set; }

        /// <summary>
        /// Gets or sets the date a task is due.
        /// </summary>
        public DateTime? DueDateTime { get; set; }

        /// <summary>
        /// Gets or sets the date a task should be reminded of.
        /// </summary>        
        public DateTime? ReminderDateTime { get; set; }

        /// <summary>
        /// Gets or sets the date a task has been completed.
        /// </summary>
        public DateTime? CompletedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the user the task is assigned to.
        /// </summary>
        public int? AssignedTo { get; set; }

        /// <summary>
        /// Gets or sets the user the task priority.
        /// </summary>
        public TaskItemPriorityEnum TaskItemPriority { get; set; }

        /// <summary>
        /// Gets or sets the user the task status.
        /// </summary>
        public TaskItemStatusEnum TaskItemStatus { get; set; }
    }
}