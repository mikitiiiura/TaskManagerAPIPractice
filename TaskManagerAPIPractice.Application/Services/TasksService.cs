using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagerAPIPractice.DataAccess;
using TaskManagerAPIPractice.DataAccess.abstruct;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.Application.Services
{
    public class TasksService : ITasksService
    {
        private readonly ITasksRepository _tasksRepository;
        private readonly TaskAPIDbContext _dbContext;
        private readonly ILogger _logger;

        public TasksService(ITasksRepository tasksRepository, TaskAPIDbContext dbContext, ILogger logger)
        {
            _tasksRepository = tasksRepository;
            _dbContext = dbContext;
            _logger = logger;
        }

        // Отримати всі завдання
        public async Task<List<TaskEntity>> GetAll()
        {
            return await _tasksRepository.GetAll();
        }

        // Отримати завдання за ID
        public async Task<TaskEntity?> GetById(Guid id)
        {
            return await _tasksRepository.GetById(id);
        }

        //// Додати завдання з повідомленням
        //public async Task AddTaskWithNotification(TaskEntity task)
        //{
        //    _logger.LogInformation("Adding new task with title: {TaskTitle}", task.Title);
        //    try
        //    {
        //        _dbContext.Tasks.Add(task);
        //        await _dbContext.SaveChangesAsync();

        //        var notifications = new List<NotificationEntity>
        //        {
        //            new NotificationEntity
        //            {
        //                Id = Guid.NewGuid(),
        //                Message = $"Task '{task.Title}' created.",
        //                CreatedAt = DateTime.UtcNow,
        //                UserId = task.TaskCreatedById,
        //                TaskId = task.Id
        //            }
        //        };

        //        if (task.TaskAssignedToId != null)
        //        {
        //            notifications.Add(new NotificationEntity
        //            {
        //                Id = Guid.NewGuid(),
        //                Message = $"New task '{task.Title}' assigned to you.",
        //                CreatedAt = DateTime.UtcNow,
        //                UserId = task.TaskAssignedToId.Value,
        //                TaskId = task.Id
        //            });
        //        }

        //        _dbContext.Notifications.AddRange(notifications);
        //        await _dbContext.SaveChangesAsync();
        //        _logger.LogInformation("Successfully added task with title: {TaskTitle}", task.Title);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error occurred while adding task with title: {TaskTitle}", task.Title);
        //        throw;
        //    }
        //}


        // Оновити завдання
        public async Task Update(TaskEntity task)
        {
            _logger.LogInformation("Updating task with ID: {TaskId}", task.Id);
            try
            {
                await _tasksRepository.Update(task);
                await _dbContext.SaveChangesAsync();
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
                await _tasksRepository.Delete(id);
                _logger.LogInformation("Successfully deleted task with ID: {TaskId}", id);
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
            return await _tasksRepository.GetFilteredTasks(userId, search, status, priority, deadline, project, tag);
        }

        public async Task UpdateStatus(Guid id, TaskManagerAPIPractice.Core.Model.TaskStatus status)
        {
            await _tasksRepository.UpdateStatus(id, status);
        }

        public async Task UpdatePriority(Guid id, TaskManagerAPIPractice.Core.Model.TaskPriority priority)
        {
            await _tasksRepository.UpdatePriority(id, priority);
        }

        public async Task<List<TaskEntity>> GetAllByUser(Guid userId)
        {
            return await _tasksRepository.GetAllByUser(userId);
        }
    }
}