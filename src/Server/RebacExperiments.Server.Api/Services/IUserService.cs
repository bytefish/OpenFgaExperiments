// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RebacExperiments.Server.Database.Models;
using System.Security.Claims;

namespace RebacExperiments.Server.Api.Services
{
    /// <summary>
    /// An <see cref="IUserService"/> is responsible to get all claims associated with a user.
    /// </summary>
    public interface IUserService
    {
        Task<User> CreateUserAsync(User user, int currentUserId, CancellationToken cancellationToken);
        Task DeleteUserAsync(int userId, int currentUserId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the Claims for a given username and password.
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>A <see cref="ServiceResult"/> with the associated claims, if successful</returns>
        Task<List<Claim>> GetClaimsAsync(string username, string password, CancellationToken cancellationToken);
        Task<User> GetUserByIdAsync(int userId, int currentUserId, CancellationToken cancellationToken);
        Task<List<User>> GetUsersByUserIdAsync(int userId, CancellationToken cancellationToken);
        Task<User> UpdateUserAsync(User user, int currentUserId, CancellationToken cancellationToken);
    }
}
