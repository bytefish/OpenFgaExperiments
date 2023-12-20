// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace RebacExperiments.Server.Api.Models
{
    /// <summary>
    /// Role of a User within an Organization, for example "admin", "member", ...
    /// </summary>
    public class OrganizationRole : Entity
    {
        /// <summary>
        /// Gets or sets the OrganizationId.
        /// </summary>
        public required int OrganizationId { get; set; }

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