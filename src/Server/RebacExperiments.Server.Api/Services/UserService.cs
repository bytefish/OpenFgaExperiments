// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using OpenFga.Sdk.Client;
using OpenFga.Sdk.Model;
using RebacExperiments.Server.Api.Infrastructure.Authentication;
using RebacExperiments.Server.Api.Infrastructure.Constants;
using RebacExperiments.Server.Api.Infrastructure.Exceptions;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using RebacExperiments.Server.Database;
using RebacExperiments.Server.Database.Models;
using System.Security.Claims;

namespace RebacExperiments.Server.Api.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IAclService _aclService;

        public UserService(ILogger<UserService> logger, ApplicationDbContext applicationDbContext, IPasswordHasher passwordHasher, IAclService aclService)
        {
            _logger = logger;
            _applicationDbContext = applicationDbContext;
            _passwordHasher = passwordHasher;
            _aclService = aclService;
        }

        public async Task<List<Claim>> GetClaimsAsync(string username, string password, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            var user = await _applicationDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.LogonName == username, cancellationToken);

            if(user == null)
            {
                throw new AuthenticationFailedException();
            }

            if (!user.IsPermittedToLogon)
            {
                throw new AuthenticationFailedException();
            }

            // Verify hashed password in database against the provided password
            var isVerifiedPassword = _passwordHasher.VerifyHashedPassword(user.HashedPassword, password);

            if (!isVerifiedPassword)
            {
                throw new AuthenticationFailedException();
            }

            var query = from userRole in _applicationDbContext.UserRoles
                            join role in _applicationDbContext.Roles on userRole.RoleId equals role.Id
                        where userRole.UserId.Equals(user.Id)
                        select role;

            var roles = await query
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            // Build the Claims for the ClaimsPrincipal
            var claims = CreateClaims(user, roles);

            return claims;
        }

        private List<Claim> CreateClaims(User user, List<Role> roles)
        {
            _logger.TraceMethodEntry();

            var claims = new List<Claim>();

            if (user.LogonName != null)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.LogonName));
                claims.Add(new Claim(ClaimTypes.Email, user.LogonName));
            }

            // Default Claims:
            claims.Add(new Claim(ClaimTypes.Sid, Convert.ToString(user.Id)));
            claims.Add(new Claim(ClaimTypes.Name, Convert.ToString(user.PreferredName)));

            // Roles:
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            return claims;
        }

        public async Task<User> CreateUserAsync(User user, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            // Make sure the Current User is the last editor:
            user.LastEditedBy = currentUserId;

            // Add the new Task, the HiLo Pattern automatically assigns a new Id using the HiLo Pattern
            await _applicationDbContext
                .AddAsync(user, cancellationToken)
                .ConfigureAwait(false);

            await _applicationDbContext
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            return user;
        }

        public async Task<User> GetUserByIdAsync(int userId, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            var user = await _applicationDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

            if (user == null)
            {
                throw new EntityNotFoundException()
                {
                    EntityName = nameof(User),
                    EntityId = userId,
                };
            }

            bool isAuthorized = await _aclService.CheckUserObjectAsync(currentUserId, user, Actions.CanRead, cancellationToken);

            if (!isAuthorized)
            {
                throw new EntityUnauthorizedAccessException()
                {
                    EntityName = nameof(User),
                    EntityId = userId,
                    UserId = currentUserId,
                };
            }

            return user;
        }

        public async Task<List<User>> GetUsersByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            var users = await _aclService
                .ListUserObjectsAsync<User>(userId, Relations.Viewer, cancellationToken)
                .ConfigureAwait(false);

            return users;
        }

        public async Task<User> UpdateUserAsync(User user, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            bool isAdministrator = await _aclService
                .CheckUserRoleAsync(user.Id, Roles.Administrator, cancellationToken)
                .ConfigureAwait(false);

            if (!isAdministrator)
            {
                throw new EntityUnauthorizedAccessException()
                {
                    EntityName = nameof(User),
                    EntityId = user.Id,
                    UserId = currentUserId,
                };
            }

            int rowsAffected = await _applicationDbContext.Users
                .Where(t => t.Id == user.Id && t.RowVersion == user.RowVersion)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.FullName, user.FullName)
                    .SetProperty(x => x.PreferredName, user.PreferredName)
                    .SetProperty(x => x.IsPermittedToLogon, user.IsPermittedToLogon)
                    .SetProperty(x => x.LogonName, user.LogonName)
                    .SetProperty(x => x.HashedPassword, user.HashedPassword)
                    .SetProperty(x => x.LastEditedBy, currentUserId), cancellationToken);

            if (rowsAffected == 0)
            {
                throw new EntityConcurrencyException()
                {
                    EntityName = nameof(User),
                    EntityId = user.Id,
                };
            }

            return user;
        }

        public async Task DeleteUserAsync(int userId, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            var user = await _applicationDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

            if (user == null)
            {
                throw new EntityNotFoundException()
                {
                    EntityName = nameof(User),
                    EntityId = userId,
                };
            }

            bool isAdministrator = await _aclService
                .CheckUserRoleAsync(userId, Roles.Administrator, cancellationToken)
                .ConfigureAwait(false);

            if (!isAdministrator)
            {
                throw new EntityUnauthorizedAccessException()
                {
                    EntityName = nameof(User),
                    EntityId = userId,
                    UserId = currentUserId,
                };
            }

            await _applicationDbContext.Users
                .Where(t => t.Id == userId)
                .ExecuteDeleteAsync(cancellationToken)
                .ConfigureAwait(false);

            var tuplesToDelete = await _aclService
                .ReadAllRelationshipsByObjectAsync<User>(userId)
                .ConfigureAwait(false);

            await _aclService.DeleteRelationshipsAsync(tuplesToDelete, cancellationToken);
        }
    }
}