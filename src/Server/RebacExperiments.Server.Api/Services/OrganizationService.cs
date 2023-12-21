using Microsoft.EntityFrameworkCore;
using RebacExperiments.Server.Api.Infrastructure.Constants;
using RebacExperiments.Server.Api.Infrastructure.Database;
using RebacExperiments.Server.Api.Infrastructure.Exceptions;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using RebacExperiments.Server.Api.Models;

namespace RebacExperiments.Server.Api.Services
{
    public class OrganizationService
    {
        private readonly ILogger<OrganizationService> _logger;

        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly IAclService _aclService;

        public OrganizationService(ILogger<OrganizationService> logger, IDbContextFactory<ApplicationDbContext> dbContextFactory, IAclService aclService)
        {
            _logger = logger;
            _dbContextFactory = dbContextFactory;
            _aclService = aclService;
        }
        
        public async Task<Organization> CreateUserTaskAsync(Organization organization, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            using (var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false))
            {
                // Make sure the Current User is the last editor:
                organization.LastEditedBy = currentUserId;

                // Add the new Task, the HiLo Pattern automatically assigns a new Id using the HiLo Pattern
                await context
                    .AddAsync(organization, cancellationToken)
                    .ConfigureAwait(false);

                // The User creating the Organization should automatically be the Owner:
                var organizationRole = new OrganizationRole
                {
                    UserId = currentUserId,
                    OrganizationId = organization.Id,
                    Role = Relations.Owner,
                };

                await context
                    .AddAsync(organizationRole, cancellationToken)
                    .ConfigureAwait(false);

                await context
                    .SaveChangesAsync(cancellationToken)
                    .ConfigureAwait(false);

                // Add to ACL Store
                await _aclService
                    .AddRelationshipAsync<Organization, User>(organization.Id, Relations.Owner, currentUserId, null)
                    .ConfigureAwait(false);

                return organization;
            }
        }

        public async Task<List<Organization>> GetOrganizationsByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            using (var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false))
            {
                var organizations = await _aclService
                    .ListUserObjectsAsync<Organization>(userId, Relations.Viewer, cancellationToken)
                    .ConfigureAwait(false);

                return organizations;
            }
        }

        public async Task<Organization> UpdateTaskItemAsync(Organization organization, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            using (var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false))
            {
                bool isAuthorized = await _aclService.CheckUserObjectAsync(currentUserId, organization, Relations.Owner, cancellationToken);

                if (!isAuthorized)
                {
                    throw new EntityUnauthorizedAccessException()
                    {
                        EntityName = nameof(Organization),
                        EntityId = organization.Id,
                        UserId = currentUserId,
                    };
                }

                int rowsAffected = await context.Organizations
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
        }

        public async Task DeleteOrganizationAsync(int organizationId, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            using (var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false))
            {
                var organization = await context.Organizations
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

                bool isAuthorized = await _aclService.CheckUserObjectAsync<TaskItem>(currentUserId, organizationId, Relations.Owner, cancellationToken);

                if (!isAuthorized)
                {
                    throw new EntityUnauthorizedAccessException()
                    {
                        EntityName = nameof(Organization),
                        EntityId = organizationId,
                        UserId = currentUserId,
                    };
                }

                using(var transaction = await context.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false))
                {
                    await context.OrganizationRoles
                        .Where(t => t.Id == organization.Id)
                        .ExecuteDeleteAsync(cancellationToken)
                        .ConfigureAwait(false);

                    await context.Organizations
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
        }

        public async Task<OrganizationRole> AddUserToOrganizationAsync(int organizationId, int userId, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            using (var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false))
            {
                bool isAuthorized = await _aclService.CheckUserObjectAsync<Organization>(currentUserId, organizationId, Relations.Owner, cancellationToken);

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

                await context
                    .AddAsync(organizationRole)
                    .ConfigureAwait(false);
             
                await context
                    .SaveChangesAsync(cancellationToken)
                    .ConfigureAwait(false);

                await _aclService
                    .AddRelationshipAsync<Organization, User>(organizationId, Relations.Member, userId, null)
                    .ConfigureAwait(false);

                return organizationRole;
            }
        }

        public async Task<OrganizationRole> RemoveUserFromOrganizationAsync(int organizationId, int userId, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            using (var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false))
            {
                bool isAuthorized = await _aclService.CheckUserObjectAsync<Organization>(currentUserId, organizationId, Relations.Owner, cancellationToken);

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

                await context
                    .AddAsync(organizationRole)
                    .ConfigureAwait(false);

                await context
                    .SaveChangesAsync(cancellationToken)
                    .ConfigureAwait(false);

                await _aclService
                    .DeleteRelationshipAsync<Organization, User>(organizationId, Relations.Member, userId, null, cancellationToken)
                    .ConfigureAwait(false);

                return organizationRole;
            }
        }


    }
}
