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

        public async Task<TaskItem> CreateUserTaskAsync(TaskItem taskItem, int currentUserId, CancellationToken cancellationToken)
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

                return taskItem;
            }
        }

        public async Task<TaskItem> GetTaskIemByUserIdAsync(int userTaskId, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            using (var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false))
            {
                var taskItem = await context.UserTasks
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userTaskId, cancellationToken);

                if (taskItem == null)
                {
                    throw new EntityNotFoundException()
                    {
                        EntityName = nameof(TaskItem),
                        EntityId = userTaskId,
                    };
                }

                bool isAuthorized = await _aclService.CheckUserObjectAsync(currentUserId, taskItem, Relations.Viewer, cancellationToken);

                if (!isAuthorized)
                {
                    throw new EntityUnauthorizedAccessException()
                    {
                        EntityName = nameof(TaskItem),
                        EntityId = userTaskId,
                        UserId = currentUserId,
                    };
                }

                return taskItem;
            }
        }

        public async Task<List<TaskItem>> GetTasksItemsAsync(int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            using (var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false))
            {
                var userTasks = await _aclService
                    .ListUserObjectsAsync<TaskItem>(currentUserId, Relations.Viewer, cancellationToken)
                    .ConfigureAwait(false);

                return userTasks;
            }
        }

        public async Task<TaskItem> UpdateTaskItemAsync(TaskItem userTask, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            using (var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false))
            {
                bool isAuthorized = await _aclService.CheckUserObjectAsync(currentUserId, userTask, Relations.Owner, cancellationToken);

                if (!isAuthorized)
                {
                    throw new EntityUnauthorizedAccessException()
                    {
                        EntityName = nameof(TaskItem),
                        EntityId = userTask.Id,
                        UserId = currentUserId,
                    };
                }

                int rowsAffected = await context.UserTasks
                    .Where(t => t.Id == userTask.Id && t.RowVersion == userTask.RowVersion)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.Title, userTask.Title)
                        .SetProperty(x => x.Description, userTask.Description)
                        .SetProperty(x => x.DueDateTime, userTask.DueDateTime)
                        .SetProperty(x => x.CompletedDateTime, userTask.CompletedDateTime)
                        .SetProperty(x => x.ReminderDateTime, userTask.ReminderDateTime)
                        .SetProperty(x => x.AssignedTo, userTask.AssignedTo)
                        .SetProperty(x => x.TaskItemPriority, userTask.TaskItemPriority)
                        .SetProperty(x => x.TaskItemStatus, userTask.TaskItemStatus)
                        .SetProperty(x => x.LastEditedBy, currentUserId), cancellationToken);

                if (rowsAffected == 0)
                {
                    throw new EntityConcurrencyException()
                    {
                        EntityName = nameof(TaskItem),
                        EntityId = userTask.Id,
                    };
                }

                return userTask;
            }
        }

        public async Task DeleteTaskItemAsync( int userTaskId, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            using (var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false))
            {
                var userTask = await context.UserTasks
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userTaskId, cancellationToken);

                if (userTask == null)
                {
                    throw new EntityNotFoundException()
                    {
                        EntityName = nameof(TaskItem),
                        EntityId = userTaskId,
                    };
                }

                bool isAuthorized = await _aclService.CheckUserObjectAsync<TaskItem>(currentUserId, userTaskId, Relations.Owner, cancellationToken);

                if (!isAuthorized)
                {
                    throw new EntityUnauthorizedAccessException()
                    {
                        EntityName = nameof(TaskItem),
                        EntityId = userTaskId,
                        UserId = currentUserId,
                    };
                }

                // After removing all possible references, delete the UserTask itself
                int rowsAffected = await context.UserTasks
                    .Where(t => t.Id == userTask.Id)
                    .ExecuteDeleteAsync(cancellationToken);

                // No Idea if this could happen, because we are in a Transaction and there
                // is a row, which should be locked. So this shouldn't happen at all...
                if (rowsAffected == 0)
                {
                    throw new EntityConcurrencyException()
                    {
                        EntityName = nameof(TaskItem),
                        EntityId = userTaskId,
                    };
                }
            }
        }
    }
}