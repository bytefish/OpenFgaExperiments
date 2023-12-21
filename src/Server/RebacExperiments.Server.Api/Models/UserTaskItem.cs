// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace RebacExperiments.Server.Api.Models
{
    public class TaskItemItem : Entity
    {
        /// <summary>
        /// Gets or sets the UserId.
        /// </summary>
        public required int UserId { get; set; }

        /// <summary>
        /// Gets or sets the TaskItemId.
        /// </summary>
        public required int TaskItemId { get; set; }

        /// <summary>
        /// Gets or sets the Role.
        /// </summary>
        public required string Role { get; set; }
    }
}
