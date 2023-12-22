// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace RebacExperiments.Server.Database.Models
{
    /// <summary>
    /// Role of the User within a Team.
    /// </summary>
    public class TeamRole : Entity
    {
        /// <summary>
        /// Gets or sets the TeamId.
        /// </summary>
        public required int TeamId { get; set; }

        /// <summary>
        /// Gets or sets the UserId.
        /// </summary>
        public required int UserId { get; set; }

        /// <summary>
        /// Gets or sets the Role.
        /// </summary>
        public required string Role { get; set; }
    }
}
