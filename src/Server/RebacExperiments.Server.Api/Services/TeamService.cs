// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using RebacExperiments.Server.Api.Infrastructure.Constants;
using RebacExperiments.Server.Api.Infrastructure.Exceptions;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using RebacExperiments.Server.Database;
using RebacExperiments.Server.Database.Models;

namespace RebacExperiments.Server.Api.Services
{
    public class TeamService : ITeamService
    {
        private readonly ILogger<TeamService> _logger;

        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IAclService _aclService;

        public TeamService(ILogger<TeamService> logger, ApplicationDbContext applicationDbContext, IAclService aclService)
        {
            _logger = logger;
            _applicationDbContext = applicationDbContext;
            _aclService = aclService;
        }

        public async Task<Team> CreateTeamAsync(Team team, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            // Make sure the Current User is the last editor:
            team.LastEditedBy = currentUserId;

            // Add the new Task, the HiLo Pattern automatically assigns a new Id using the HiLo Pattern
            await _applicationDbContext
                .AddAsync(team, cancellationToken)
                .ConfigureAwait(false);

            // The User creating the Organization should automatically be the Owner:
            var teamRole = new TeamRole
            {
                UserId = currentUserId,
                TeamId = team.Id,
                Role = Relations.Owner,
                LastEditedBy = currentUserId,
            };

            await _applicationDbContext
                .AddAsync(teamRole, cancellationToken)
                .ConfigureAwait(false);

            await _applicationDbContext
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            // Add to ACL Store
            await _aclService
                .AddRelationshipAsync<Team, User>(team.Id, Relations.Owner, currentUserId, null)
                .ConfigureAwait(false);

            return team;
        }

        public async Task<Team> GetTeamByIdAsync(int teamId, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            var team = await _applicationDbContext.Teams
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == teamId, cancellationToken);

            if (team == null)
            {
                throw new EntityNotFoundException()
                {
                    EntityName = nameof(Team),
                    EntityId = teamId,
                };
            }

            bool isAuthorized = await _aclService.CheckUserObjectAsync(currentUserId, team, Actions.CanRead, cancellationToken);

            if (!isAuthorized)
            {
                throw new EntityUnauthorizedAccessException()
                {
                    EntityName = nameof(Team),
                    EntityId = teamId,
                    UserId = currentUserId,
                };
            }

            return team;
        }


        public async Task<List<Team>> GetTeamsByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            var teams = await _aclService
                .ListUserObjectsAsync<Team>(userId, Relations.Viewer, cancellationToken)
                .ConfigureAwait(false);

            return teams;
        }

        public async Task<Team> UpdateTeamAsync(Team team, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            bool isAuthorized = await _aclService.CheckUserObjectAsync(currentUserId, team, Actions.CanWrite, cancellationToken);

            if (!isAuthorized)
            {
                throw new EntityUnauthorizedAccessException()
                {
                    EntityName = nameof(Team),
                    EntityId = team.Id,
                    UserId = currentUserId,
                };
            }

            int rowsAffected = await _applicationDbContext.Teams
                .Where(t => t.Id == team.Id && t.RowVersion == team.RowVersion)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.Name, team.Name)
                    .SetProperty(x => x.Description, team.Description)
                    .SetProperty(x => x.LastEditedBy, currentUserId), cancellationToken);

            if (rowsAffected == 0)
            {
                throw new EntityConcurrencyException()
                {
                    EntityName = nameof(TaskItem),
                    EntityId = team.Id,
                };
            }

            return team;

        }

        public async Task DeleteTeamAsync(int teamId, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            var team = await _applicationDbContext.Teams
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == teamId, cancellationToken);

            if (team == null)
            {
                throw new EntityNotFoundException()
                {
                    EntityName = nameof(Team),
                    EntityId = teamId,
                };
            }

            bool isAuthorized = await _aclService.CheckUserObjectAsync<TaskItem>(currentUserId, teamId, Actions.CanWrite, cancellationToken);

            if (!isAuthorized)
            {
                throw new EntityUnauthorizedAccessException()
                {
                    EntityName = nameof(Team),
                    EntityId = teamId,
                    UserId = currentUserId,
                };
            }

            using (var transaction = await _applicationDbContext.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false))
            {
                // Remove all User Assignements to the Team
                await _applicationDbContext.TeamRoles
                    .Where(t => t.Id == team.Id)
                    .ExecuteDeleteAsync(cancellationToken)
                    .ConfigureAwait(false);

                // After removing all possible references, delete the TaskItem itself
                await _applicationDbContext.Teams
                    .Where(t => t.Id == teamId)
                    .ExecuteDeleteAsync(cancellationToken)
                    .ConfigureAwait(false);

                await transaction
                    .CommitAsync(cancellationToken)
                    .ConfigureAwait(false);
            }

            // Delete all Relations towards the Team
            var tuplesToDelete = await _aclService
                .ReadAllRelationshipsByObjectAsync<Team>(teamId)
                .ConfigureAwait(false);

            await _aclService.DeleteRelationshipsAsync(tuplesToDelete, cancellationToken);
        }

        public async Task<TeamRole> AddUserToTeamAsync(int teamId, int userId, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            bool isAuthorized = await _aclService.CheckUserObjectAsync<Team>(currentUserId, teamId, Actions.CanWrite, cancellationToken);

            if (!isAuthorized)
            {
                throw new EntityUnauthorizedAccessException()
                {
                    EntityName = nameof(Team),
                    EntityId = teamId,
                    UserId = currentUserId,
                };
            }

            var teamRole = new TeamRole
            {
                TeamId = teamId,
                UserId = userId,
                Role = Relations.Member,
                LastEditedBy = currentUserId,
            };

            await _applicationDbContext
                .AddAsync(teamRole)
                .ConfigureAwait(false);

            await _applicationDbContext
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            await _aclService
                .AddRelationshipAsync<Team, User>(teamId, Relations.Member, userId, null)
                .ConfigureAwait(false);

            return teamRole;
        }

        public async Task RemoveUserFromTeamAsync(int teamId, int userId, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            bool isAuthorized = await _aclService.CheckUserObjectAsync<Team>(currentUserId, teamId, Actions.CanWrite, cancellationToken);

            if (!isAuthorized)
            {
                throw new EntityUnauthorizedAccessException()
                {
                    EntityName = nameof(Team),
                    EntityId = teamId,
                    UserId = currentUserId,
                };
            }

            await _applicationDbContext.TeamRoles
                .Where(x => x.TeamId == teamId && x.UserId == userId)
                .ExecuteDeleteAsync(cancellationToken)
                .ConfigureAwait(false);

            await _aclService
                .DeleteRelationshipAsync<Team, User>(teamId, Relations.Member, userId, null, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}