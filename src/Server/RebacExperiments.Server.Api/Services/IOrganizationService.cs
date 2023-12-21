using RebacExperiments.Server.Api.Models;

namespace RebacExperiments.Server.Api.Services
{
    public interface IOrganizationService
    {
        Task<Organization> CreateUserTaskAsync(Organization organization, int currentUserId, CancellationToken cancellationToken);

        Task<List<Organization>> GetOrganizationsByUserIdAsync(int userId, CancellationToken cancellationToken);

        Task<Organization> UpdateTaskItemAsync(Organization organization, int currentUserId, CancellationToken cancellationToken);

        Task DeleteOrganizationAsync(int organizationId, int currentUserId, CancellationToken cancellationToken);

        Task<OrganizationRole> AddUserToOrganizationAsync(int organizationId, int userId, int currentUserId, CancellationToken cancellationToken);
    }
}