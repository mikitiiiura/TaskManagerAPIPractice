using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagerAPIPractice.DataAccess.abstruct;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.DataAccess.Repositories
{
    public class TasksRepository : ITasksRepository
    {
        private readonly TaskAPIDbContext _context;
        private readonly ILogger<TasksRepository> _logger;

        public TasksRepository(TaskAPIDbContext context, ILogger<TasksRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Отримати всі завдання
        public async Task<List<TaskEntity>> GetAll()
        {
            _logger.LogInformation("Fetching all tasks.");
            try
            {
                var tasks = await _context.Tasks
                    .Include(t => t.TaskCreatedBy)
                    .Include(t => t.TaskAssignedTo)
                    .Include(t => t.Category)
                    .Include(t => t.Project)
                    .Include(t => t.Tags)
                    .Include(t => t.Notifications)
                    .ToListAsync();
                _logger.LogInformation("Successfully fetched all tasks.");
                return tasks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all tasks.");
                throw;
            }
        }

        // Отримати завдання за ID
        public async Task<TaskEntity?> GetById(Guid id)
        {
            _logger.LogInformation("Fetching task with ID: {TaskId}", id);
            try
            {
                var task = await _context.Tasks
                    .Include(t => t.TaskCreatedBy)
                    .Include(t => t.TaskAssignedTo)
                    .Include(t => t.Category)
                    .Include(t => t.Project)
                    .Include(t => t.Tags)
                    .Include(t => t.Notifications)
                    .FirstOrDefaultAsync(t => t.Id == id) ?? throw new Exception("Task not found");
                _logger.LogInformation("Successfully fetched task with ID: {TaskId}", id);
                return task;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching task with ID: {TaskId}", id);
                throw;
            }
        }

        // Додати завдання
        public async Task Add(TaskEntity task)
        {
            _logger.LogInformation("Adding new task with title: {TaskTitle}", task.Title);
            try
            {
                await _context.Tasks.AddAsync(task);
                await _context.SaveChangesAsync();

                var notifications = new List<NotificationEntity>
                {
                    new NotificationEntity
                    {
                        Id = Guid.NewGuid(),
                        Message = $"Task '{task.Title}' created.",
                        CreatedAt = DateTime.UtcNow,
                        UserId = task.TaskCreatedById,
                        TaskId = task.Id
                    }
                };

                _context.Notifications.AddRange(notifications);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully added task with title: {TaskTitle}", task.Title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding task with title: {TaskTitle}", task.Title);
                throw;
            }
        }

        // Оновити завдання
        public async Task Update(TaskEntity task)
        {
            _logger.LogInformation("Updating task with ID: {TaskId}", task.Id);
            try
            {
                var existingTask = await _context.Tasks
                    .Include(t => t.Tags)
                    .Include(t => t.Notifications)
                    .FirstOrDefaultAsync(t => t.Id == task.Id);

                if (existingTask == null)
                {
                    _logger.LogWarning("Task with ID {TaskId} not found.", task.Id);
                    throw new Exception("Task not found");
                }

                // Оновлення основних полів
                existingTask.Title = task.Title;
                existingTask.Description = task.Description;
                existingTask.Status = task.Status;
                existingTask.Priority = task.Priority;
                existingTask.DeadLine = task.DeadLine;
                existingTask.CreatedAt = task.CreatedAt;

                // Оновлення зовнішніх ключів
                existingTask.TaskCreatedById = task.TaskCreatedById;
                existingTask.TaskAssignedToId = task.TaskAssignedToId;
                existingTask.CategoryId = task.CategoryId;
                existingTask.ProjectId = task.ProjectId;
                existingTask.TeamId = task.TeamId;

                // Оновлення навігаційних властивостей
                existingTask.Tags = task.Tags;
                existingTask.Notifications = task.Notifications;

                _context.Tasks.Update(existingTask);
                await _context.SaveChangesAsync();

                var notifications = new List<NotificationEntity>
                {
                    new NotificationEntity
                    {
                        Id = Guid.NewGuid(),
                        Message = $"Task '{task.Title}' updated.",
                        CreatedAt = DateTime.UtcNow,
                        UserId = task.TaskCreatedById,
                        TaskId = task.Id
                    }
                };

                _context.Notifications.AddRange(notifications);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully updated task with ID: {TaskId}", task.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating task with ID: {TaskId}", task.Id);
                throw;
            }
        }

        // Видалити завдання
        public async Task Delete(Guid id)
        {
            _logger.LogInformation("Deleting task with ID: {TaskId}", id);
            try
            {
                var task = await _context.Tasks
                    .Include(t => t.Notifications)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (task != null)
                {
                    var notification = new NotificationEntity
                    {
                        Id = Guid.NewGuid(),
                        Message = $"Task '{task.Title}' deleted.",
                        CreatedAt = DateTime.UtcNow,
                        UserId = task.TaskCreatedById,
                    };

                    _context.Notifications.Add(notification);
                    await _context.SaveChangesAsync();

                    _context.Notifications.RemoveRange(task.Notifications);
                    _context.Tasks.Remove(task);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Successfully deleted task with ID: {TaskId}", id);
                }
                else
                {
                    _logger.LogWarning("Task with ID {TaskId} not found.", id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting task with ID: {TaskId}", id);
                throw;
            }
        }

        // Отримати відфільтровані завдання
        public async Task<List<TaskEntity>> GetFilteredTasks(Guid userId, string? search, int? status, int? priority, DateTime? deadline, string? project, string? tag)
        {
            _logger.LogInformation("Fetching filtered tasks for user ID: {UserId}", userId);
            try
            {
                var query = _context.Tasks
                    .Include(t => t.TaskCreatedBy)
                    .Include(t => t.TaskAssignedTo)
                    .Include(t => t.Category)
                    .Include(t => t.Project)
                    .Include(t => t.Tags)
                    .Include(t => t.Notifications)
                    .Where(t => t.TaskCreatedById == userId || t.TaskAssignedToId == userId)
                    .AsNoTracking()
                    .AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(t => EF.Functions.Like(t.Title, $"%{search}%") || EF.Functions.Like(t.Description, $"%{search}%"));
                }

                if (status.HasValue)
                {
                    query = query.Where(t => (int)t.Status == status.Value);
                }

                if (priority.HasValue)
                {
                    query = query.Where(t => (int)t.Priority == priority.Value);
                }

                if (deadline.HasValue)
                {
                    query = query.Where(t => t.DeadLine.HasValue && t.DeadLine.Value.Date == deadline.Value.Date);
                }

                if (!string.IsNullOrEmpty(project))
                {
                    query = query.Where(t => t.Project != null && EF.Functions.Like(t.Project.Title, $"%{project}%"));
                }

                if (!string.IsNullOrEmpty(tag))
                {
                    query = query.Where(t => t.Tags != null && t.Tags.Any(tagEntity => EF.Functions.Like(tagEntity.Name, $"%{tag}%")));
                }

                var tasks = await query.ToListAsync();
                _logger.LogInformation("Successfully fetched filtered tasks for user ID: {UserId}", userId);
                return tasks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching filtered tasks for user ID: {UserId}", userId);
                throw;
            }
        }

        // Оновити статус завдання
        public async Task UpdateStatus(Guid id, TaskManagerAPIPractice.Core.Model.TaskStatus status)
        {
            _logger.LogInformation("Updating status for task with ID: {TaskId}", id);
            try
            {
                var existingTask = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
                if (existingTask == null)
                {
                    _logger.LogWarning("Task with ID {TaskId} not found.", id);
                    throw new Exception("Task not found");
                }

                existingTask.Status = status;
                _context.Entry(existingTask).Property(x => x.Status).IsModified = true;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully updated status for task with ID: {TaskId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating status for task with ID: {TaskId}", id);
                throw;
            }
        }

        // Оновити пріоритет завдання
        public async Task UpdatePriority(Guid id, TaskManagerAPIPractice.Core.Model.TaskPriority priority)
        {
            _logger.LogInformation("Updating priority for task with ID: {TaskId}", id);
            try
            {
                var existingTask = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
                if (existingTask == null)
                {
                    _logger.LogWarning("Task with ID {TaskId} not found.", id);
                    throw new Exception("Task not found");
                }

                existingTask.Priority = priority;
                _context.Entry(existingTask).Property(x => x.Priority).IsModified = true;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully updated priority for task with ID: {TaskId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating priority for task with ID: {TaskId}", id);
                throw;
            }
        }

        // Отримати всі завдання користувача
        public async Task<List<TaskEntity>> GetAllByUser(Guid userId)
        {
            _logger.LogInformation("Fetching all tasks for user ID: {UserId}", userId);
            try
            {
                var tasks = await _context.Tasks
                    .Where(t => t.TaskCreatedById == userId || t.TaskAssignedToId == userId)
                    .Include(t => t.TaskCreatedBy)
                    .Include(t => t.TaskAssignedTo)
                    .Include(t => t.Category)
                    .Include(t => t.Project)
                    .Include(t => t.Tags)
                    .Include(t => t.Notifications)
                    .AsNoTracking()
                    .ToListAsync();
                _logger.LogInformation("Successfully fetched all tasks for user ID: {UserId}", userId);
                return tasks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all tasks for user ID: {UserId}", userId);
                throw;
            }
        }
    }
}