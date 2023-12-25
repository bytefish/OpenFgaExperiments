// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RebacExperiments.Server.Database.Models;

namespace RebacExperiments.Server.Api.Services
{
    public interface IOrganizationService
    {
        Task<OrganizationRole> AddUserToOrganizationAsync(int organizationId, int userId, int currentUserId, CancellationToken cancellationToken);
        Task<Organization> CreateOrganizationAsync(Organization organization, int currentUserId, CancellationToken cancellationToken);
        Task DeleteOrganizationAsync(int organizationId, int currentUserId, CancellationToken cancellationToken);
        Task<Organization> GetOrganizationByIdAsync(int organizationId, int currentUserId, CancellationToken cancellationToken);
        Task<List<Organization>> GetOrganizationsByUserIdAsync(int userId, CancellationToken cancellationToken);
        Task<OrganizationRole> RemoveUserFromOrganizationAsync(int organizationId, int userId, int currentUserId, CancellationToken cancellationToken);
        Task<Organization> UpdateOrganizationAsync(Organization organization, int currentUserId, CancellationToken cancellationToken);
    }
}