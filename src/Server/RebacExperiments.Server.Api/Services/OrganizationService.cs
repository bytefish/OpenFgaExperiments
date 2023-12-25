// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using RebacExperiments.Server.Api.Infrastructure.Constants;
using RebacExperiments.Server.Api.Infrastructure.Exceptions;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using RebacExperiments.Server.Database;
using RebacExperiments.Server.Database.Models;

namespace RebacExperiments.Server.Api.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly ILogger<OrganizationService> _logger;

        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IAclService _aclService;

        public OrganizationService(ILogger<OrganizationService> logger, ApplicationDbContext applicationDbContext, IAclService aclService)
        {
            _logger = logger;
            _applicationDbContext = applicationDbContext;
            _aclService = aclService;
        }

        public async Task<Organization> CreateOrganizationAsync(Organization organization, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            // Make sure the Current User is the last editor:
            organization.LastEditedBy = currentUserId;

            // Add the new Task, the HiLo Pattern automatically assigns a new Id using the HiLo Pattern
            await _applicationDbContext
                .AddAsync(organization, cancellationToken)
                .ConfigureAwait(false);

            // The User creating the Organization should automatically be the Owner:
            var organizationRole = new OrganizationRole
            {
                UserId = currentUserId,
                OrganizationId = organization.Id,
                Role = Relations.Owner,
                LastEditedBy = currentUserId
            };

            await _applicationDbContext
                .AddAsync(organizationRole, cancellationToken)
                .ConfigureAwait(false);

            await _applicationDbContext
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            // Add to ACL Store
            await _aclService
                .AddRelationshipAsync<Organization, User>(organization.Id, Relations.Owner, currentUserId, null)
                .ConfigureAwait(false);

            return organization;
        }

        public async Task<Organization> GetOrganizationByIdAsync(int organizationId, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            var organization = await _applicationDbContext.Organizations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == organizationId, cancellationToken);

            if (organization == null)
            {
                throw new EntityNotFoundException()
                {
                    EntityName = nameof(Organization),
                    EntityId = organizationId,
                };
            }

            bool isAuthorized = await _aclService.CheckUserObjectAsync(currentUserId, organization, Actions.CanRead, cancellationToken);

            if (!isAuthorized)
            {
                throw new EntityUnauthorizedAccessException()
                {
                    EntityName = nameof(Organization),
                    EntityId = organizationId,
                    UserId = currentUserId,
                };
            }

            return organization;
        }

        public async Task<List<Organization>> GetOrganizationsByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            var organizations = await _aclService
                .ListUserObjectsAsync<Organization>(userId, Actions.CanRead, cancellationToken)
                .ConfigureAwait(false);

            return organizations;
        }

        public async Task<Organization> UpdateOrganizationAsync(Organization organization, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            bool isAuthorized = await _aclService.CheckUserObjectAsync(currentUserId, organization, Actions.CanWrite, cancellationToken);

            if (!isAuthorized)
            {
                throw new EntityUnauthorizedAccessException()
                {
                    EntityName = nameof(Organization),
                    EntityId = organization.Id,
                    UserId = currentUserId,
                };
            }

            int rowsAffected = await _applicationDbContext.Organizations
                .Where(t => t.Id == organization.Id && t.RowVersion == organization.RowVersion)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.Name, organization.Name)
                    .SetProperty(x => x.Description, organization.Description)
                    .SetProperty(x => x.LastEditedBy, currentUserId), cancellationToken);

            if (rowsAffected == 0)
            {
                throw new EntityConcurrencyException()
                {
                    EntityName = nameof(Organization),
                    EntityId = organization.Id,
                };
            }

            return organization;
        }

        public async Task DeleteOrganizationAsync(int organizationId, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            var organization = await _applicationDbContext.Organizations
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == organizationId, cancellationToken);

            if (organization == null)
            {
                throw new EntityNotFoundException()
                {
                    EntityName = nameof(Organization),
                    EntityId = organizationId,
                };
            }

            bool isAuthorized = await _aclService.CheckUserObjectAsync<TaskItem>(currentUserId, organizationId, Actions.CanWrite, cancellationToken);

            if (!isAuthorized)
            {
                throw new EntityUnauthorizedAccessException()
                {
                    EntityName = nameof(Organization),
                    EntityId = organizationId,
                    UserId = currentUserId,
                };
            }

            using (var transaction = await _applicationDbContext.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false))
            {
                await _applicationDbContext.OrganizationRoles
                    .Where(t => t.Id == organization.Id)
                    .ExecuteDeleteAsync(cancellationToken)
                    .ConfigureAwait(false);

                await _applicationDbContext.Organizations
                    .Where(t => t.Id == organization.Id)
                    .ExecuteDeleteAsync(cancellationToken)
                    .ConfigureAwait(false);

                await transaction
                    .CommitAsync(cancellationToken)
                    .ConfigureAwait(false);
            }

            // Delete all Relations towards the Organization
            var tuplesToDelete = await _aclService
                .ReadAllRelationshipsByObjectAsync<Organization>(organizationId)
                .ConfigureAwait(false);

            await _aclService.DeleteRelationshipsAsync(tuplesToDelete, cancellationToken);
        }

        public async Task<OrganizationRole> AddUserToOrganizationAsync(int organizationId, int userId, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            bool isAuthorized = await _aclService.CheckUserObjectAsync<Organization>(currentUserId, organizationId, Actions.CanWrite, cancellationToken);

            if (!isAuthorized)
            {
                throw new EntityUnauthorizedAccessException()
                {
                    EntityName = nameof(Organization),
                    EntityId = organizationId,
                    UserId = currentUserId,
                };
            }

            var organizationRole = new OrganizationRole
            {
                OrganizationId = organizationId,
                UserId = userId,
                Role = Relations.Member,
                LastEditedBy = currentUserId,
            };

            await _applicationDbContext
                .AddAsync(organizationRole)
                .ConfigureAwait(false);

            await _applicationDbContext
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            await _aclService
                .AddRelationshipAsync<Organization, User>(organizationId, Relations.Member, userId, null)
                .ConfigureAwait(false);

            return organizationRole;
        }

        public async Task<OrganizationRole> RemoveUserFromOrganizationAsync(int organizationId, int userId, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            bool isAuthorized = await _aclService.CheckUserObjectAsync<Organization>(currentUserId, organizationId, Actions.CanWrite, cancellationToken);

            if (!isAuthorized)
            {
                throw new EntityUnauthorizedAccessException()
                {
                    EntityName = nameof(Organization),
                    EntityId = organizationId,
                    UserId = currentUserId,
                };
            }

            var organizationRole = new OrganizationRole
            {
                OrganizationId = organizationId,
                UserId = userId,
                Role = Relations.Member,
                LastEditedBy = currentUserId,
            };

            await _applicationDbContext
                .AddAsync(organizationRole)
                .ConfigureAwait(false);

            await _applicationDbContext
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            await _aclService
                .DeleteRelationshipAsync<Organization, User>(organizationId, Relations.Member, userId, null, cancellationToken)
                .ConfigureAwait(false);

            return organizationRole;
        }
    }
}
