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
        /// <summary>
        /// Creates a new <see cref="User"/>.
        /// </summary>
        /// <param name="user">User to create</param>
        /// <param name="currentUserId">User performing the action</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The created <see cref="User"/></returns>
        Task<User> CreateUserAsync(User user, int currentUserId, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a User.
        /// </summary>
        /// <param name="userId">User to delete</param>
        /// <param name="currentUserId">User executing the Action</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Task</returns>
        Task DeleteUserAsync(int userId, int currentUserId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the Claims for a given username and password.
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>A <see cref="ServiceResult"/> with the associated claims, if successful</returns>
        Task<List<Claim>> GetClaimsAsync(string username, string password, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the Claims for a given username.
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        Task<List<Claim>> GetClaimsAsync(string username, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a User by the UserID.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="currentUserId">Current User</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        Task<User> GetUserByIdAsync(int userId, int currentUserId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets all users visible to a given User ID.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>All Users the User with ID <paramref name="userId"/> can see</returns>
        Task<List<User>> GetUsersByUserIdAsync(int userId, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the User Data.
        /// </summary>
        /// <param name="user">User with updates</param>
        /// <param name="currentUserId">Current User</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Updated User with Computed Fields</returns>
        Task<User> UpdateUserAsync(User user, int currentUserId, CancellationToken cancellationToken);
    }
}
