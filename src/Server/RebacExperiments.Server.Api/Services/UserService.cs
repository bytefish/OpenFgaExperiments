﻿// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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

        public UserService(ILogger<UserService> logger, ApplicationDbContext applicationDbContext, IPasswordHasher passwordHasher)
        {
            _logger = logger;
            _applicationDbContext = applicationDbContext;
            _passwordHasher = passwordHasher;

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
    }
}