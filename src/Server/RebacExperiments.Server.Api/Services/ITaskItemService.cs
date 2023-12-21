// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RebacExperiments.Server.Api.Models;

namespace RebacExperiments.Server.Api.Services
{
    /// <summary>
    /// An <see cref="ITaskItemService"/> is responsible for authorized access to a <see cref="TaskItem"/>.
    /// </summary>
    public interface ITaskItemService
    {
        /// <summary>
        /// Creates a new <see cref="TaskItem"/> and assigns default relationships.
        /// </summary>
        /// <param name="taskItem"><see cref="TaskItem"/> with values</param>
        /// <param name="currentUserId"><see cref="User"/> ID</param>
        /// <param name="cancellationToken">CancellationToken to cancel asynchronous processing</param>
        /// <returns>The created <see cref="TaskItem"/></returns>
        Task<TaskItem> CreateUserTaskAsync(TaskItem taskItem, int currentUserId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a <see cref="TaskItem"/> by id for the current user.
        /// </summary>
        /// <param name="userTaskId"><see cref="TaskItem"/> ID</param>
        /// <param name="currentUserId"><see cref="User"/> ID</param>
        /// <param name="cancellationToken">CancellationToken to cancel asynchronous processing</param>
        /// <returns>The <see cref="TaskItem"/> for the given ID</returns>
        Task<TaskItem> GetTaskItemByIdAsync(int taskItemId, int currentUserId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets all <see cref="TaskItem"/> the given User has Viewer or Owner access to.
        /// </summary>
        /// <param name="currentUserId"><see cref="User"/> ID</param>
        /// <param name="cancellationToken">CancellationToken to cancel asynchronous processing</param>
        /// <returns>The <see cref="TaskItem"/> for the given ID</returns>
        Task<List<TaskItem>> GetTaskItemsByUserIdAsync(int userId, CancellationToken cancellationToken);

        /// <summary>
        /// Updates a <see cref="TaskItem"/> for the current user.
        /// </summary>
        /// <param name="taskItem"><see cref="TaskItem"/> with values</param>
        /// <param name="currentUserId"><see cref="User"/> ID</param>
        /// <param name="cancellationToken">CancellationToken to cancel asynchronous processing</param>
        /// <returns>The Updated <see cref="TaskItem"/></returns>
        Task<TaskItem> UpdateTaskItemAsync(TaskItem userTask, int currentUserId, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a <see cref="TaskItem"/> and all of its relationships.
        /// </summary>
        /// <param name="taskItemId"><see cref="TaskItem"/> to delete</param>
        /// <param name="currentUserId"><see cref="User"/> ID</param>
        /// <param name="cancellationToken">CancellationToken to cancel asynchronous processing</param>
        /// <returns>The Updated <see cref="TaskItem"/></returns>
        Task DeleteTaskItemAsync(int taskItemId, int currentUserId, CancellationToken cancellationToken);
    }
}