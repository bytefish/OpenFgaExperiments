// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace RebacExperiments.Server.Database.Models
{
    public class UserRole : Entity
    {
        /// <summary>
        /// Gets or sets the UserId.
        /// </summary>
        public required int UserId { get; set; }

        /// <summary>
        /// Gets or sets the AppRoleId.
        /// </summary>
        public required int RoleId { get; set; }
    }
}
