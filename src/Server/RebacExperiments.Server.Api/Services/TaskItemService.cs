// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using RebacExperiments.Server.Api.Infrastructure.Constants;
using RebacExperiments.Server.Api.Infrastructure.Database;
using RebacExperiments.Server.Api.Infrastructure.Exceptions;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using RebacExperiments.Server.Api.Models;

namespace RebacExperiments.Server.Api.Services
{
    public class TaskItemService : ITaskItemService
    {
        private readonly ILogger<TaskItemService> _logger;

        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly IAclService _aclService;

        public TaskItemService(ILogger<TaskItemService> logger, IDbContextFactory<ApplicationDbContext> dbContextFactory, IAclService aclService)
        {
            _logger = logger;
            _dbContextFactory = dbContextFactory;
            _aclService = aclService;
        }

        public async Task<TaskItem> CreateTaskItemAsync(TaskItem taskItem, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            using (var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false))
            {
                // Make sure the Current User is the last editor:
                taskItem.LastEditedBy = currentUserId;

                // Add the new Task, the HiLo Pattern automatically assigns a new Id using the HiLo Pattern
                await context
                    .AddAsync(taskItem, cancellationToken)
                    .ConfigureAwait(false);

                // The Current User should automatically be the Owner:
                var userTaskItem = new UserTaskItem {
                    UserId = currentUserId,
                    TaskItemId = taskItem.Id,
                    Role = Relations.Owner,
                };

                await context
                    .AddAsync(userTaskItem, cancellationToken)
                    .ConfigureAwait(false);

                await context.SaveChangesAsync(cancellationToken);

                // Acl
                await _aclService
                    .AddRelationshipAsync<TaskItem, User>(taskItem.Id, Relations.Owner, currentUserId, null)
                    .ConfigureAwait(false);

                return taskItem;
            }
        }

        public async Task<TaskItem> GetTaskItemByIdAsync(int taskItemId, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            using (var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false))
            {
                var taskItem = await context.TaskItems
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == taskItemId, cancellationToken);

                if (taskItem == null)
                {
                    throw new EntityNotFoundException()
                    {
                        EntityName = nameof(TaskItem),
                        EntityId = taskItemId,
                    };
                }

                bool isAuthorized = await _aclService.CheckUserObjectAsync(currentUserId, taskItem, Relations.Viewer, cancellationToken);

                if (!isAuthorized)
                {
                    throw new EntityUnauthorizedAccessException()
                    {
                        EntityName = nameof(TaskItem),
                        EntityId = taskItemId,
                        UserId = currentUserId,
                    };
                }

                return taskItem;
            }
        }

        public async Task<List<TaskItem>> GetTaskItemsByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            var taskItems = await _aclService
                .ListUserObjectsAsync<TaskItem>(userId, Relations.Viewer, cancellationToken)
                .ConfigureAwait(false);

            return taskItems;
        }

        public async Task<TaskItem> UpdateTaskItemAsync(TaskItem TaskItem, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            using (var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false))
            {
                bool isAuthorized = await _aclService.CheckUserObjectAsync(currentUserId, TaskItem, Relations.Owner, cancellationToken);

                if (!isAuthorized)
                {
                    throw new EntityUnauthorizedAccessException()
                    {
                        EntityName = nameof(TaskItem),
                        EntityId = TaskItem.Id,
                        UserId = currentUserId,
                    };
                }

                int rowsAffected = await context.TaskItems
                    .Where(t => t.Id == TaskItem.Id && t.RowVersion == TaskItem.RowVersion)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.Title, TaskItem.Title)
                        .SetProperty(x => x.Description, TaskItem.Description)
                        .SetProperty(x => x.DueDateTime, TaskItem.DueDateTime)
                        .SetProperty(x => x.CompletedDateTime, TaskItem.CompletedDateTime)
                        .SetProperty(x => x.ReminderDateTime, TaskItem.ReminderDateTime)
                        .SetProperty(x => x.AssignedTo, TaskItem.AssignedTo)
                        .SetProperty(x => x.TaskItemPriority, TaskItem.TaskItemPriority)
                        .SetProperty(x => x.TaskItemStatus, TaskItem.TaskItemStatus)
                        .SetProperty(x => x.LastEditedBy, currentUserId), cancellationToken);

                if (rowsAffected == 0)
                {
                    throw new EntityConcurrencyException()
                    {
                        EntityName = nameof(TaskItem),
                        EntityId = TaskItem.Id,
                    };
                }

                return TaskItem;
            }
        }

        public async Task DeleteTaskItemAsync( int taskItemId, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            using (var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false))
            {
                var taskItem = await context.TaskItems
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == taskItemId, cancellationToken);

                if (taskItem == null)
                {
                    throw new EntityNotFoundException()
                    {
                        EntityName = nameof(taskItem),
                        EntityId = taskItemId,
                    };
                }

                bool isAuthorized = await _aclService.CheckUserObjectAsync<TaskItem>(currentUserId, taskItemId, Relations.Owner, cancellationToken);

                if (!isAuthorized)
                {
                    throw new EntityUnauthorizedAccessException()
                    {
                        EntityName = nameof(taskItem),
                        EntityId = taskItemId,
                        UserId = currentUserId,
                    };
                }

                
                await context.TaskItems
                    .Where(t => t.Id == taskItem.Id)
                    .ExecuteDeleteAsync(cancellationToken);

                // Delete all stored relations towards the TaskItem
                var tuplesToDelete = await _aclService
                    .ReadAllRelationshipsByObjectAsync<TaskItem>(taskItemId)
                    .ConfigureAwait(false);

                await _aclService.DeleteRelationshipsAsync(tuplesToDelete, cancellationToken);
            }
        }
    }
}