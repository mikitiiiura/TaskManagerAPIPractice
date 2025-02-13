using Microsoft.EntityFrameworkCore;
using TaskManagerAPIPractice.DataAccess;
using TaskManagerAPIPractice.DataAccess.abstruct;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.Application.Services
{
    public class TasksService : ITasksService
    {
        private readonly ITasksRepository _tasksRepository;
        private readonly TaskAPIDbContext _dbContext;

        public TasksService(ITasksRepository tasksRepository, TaskAPIDbContext dbContext)
        {
            _tasksRepository = tasksRepository;
            _dbContext = dbContext;
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

        //// Додати нове завдання
        //public async Task Add(TaskEntity task)
        //{
        //    await _tasksRepository.Add(task);
        //}

        public async Task AddTaskWithNotification(TaskEntity task)
        {
            _dbContext.Tasks.Add(task);
            await _dbContext.SaveChangesAsync(); // Примушуємо EF оновити `task.Id`

            var notifications = new List<NotificationEntity>();

            // Повідомлення для автора
            notifications.Add(new NotificationEntity
            {
                Id = Guid.NewGuid(),
                Message = $"Завдання '{task.Title}' створено.",
                CreatedAt = DateTime.UtcNow,
                UserId = task.TaskCreatedById, // Надсилаємо автору
                TaskId = task.Id // Додаємо ідентифікатор завдання
            });

            // Повідомлення для виконавця, якщо він уже відомий
            if (task.TaskAssignedToId != null)
            {
                notifications.Add(new NotificationEntity
                {
                    Id = Guid.NewGuid(),
                    Message = $"Вам призначено нове завдання: '{task.Title}'.",
                    CreatedAt = DateTime.UtcNow,
                    UserId = task.TaskAssignedToId.Value // Надсилаємо виконавцю
                });
            }

            _dbContext.Notifications.AddRange(notifications);
            await _dbContext.SaveChangesAsync();
        }


        //public async Task AddTaskWithNotification(TaskEntity task)
        //{
        //    // Створюємо повідомлення про створення завдання
        //    var notification = new NotificationEntity
        //    {
        //        Id = Guid.NewGuid(),
        //        Message = $"Завдання '{task.Title}' створено для користувача {task.TaskAssignedToId}",
        //        CreatedAt = DateTime.UtcNow
        //    };

        //    // Додаємо його до списку повідомлень завдання
        //    task.Notifications.Add(notification);

        //    // Зберігаємо завдання в базу
        //    await _tasksRepository.Add(task);
        //}

        // Оновити завдання
        public async Task Update(TaskEntity task)
        {
            await _tasksRepository.Update(task);
        }

        // Видалити завдання
        public async Task Delete(Guid id)
        {
            await _tasksRepository.Delete(id);
        }

        // Отримати відфільтровані завдання
        public async Task<List<TaskEntity>> GetFilteredTasks(string? search, int? status, int? priority, DateTime? deadline, string? project, string? tag)
        {
            return await _tasksRepository.GetFilteredTasks(search, status, priority, deadline, project, tag);
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