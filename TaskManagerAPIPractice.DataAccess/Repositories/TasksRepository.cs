using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskManagerAPIPractice.DataAccess.abstruct;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.DataAccess.Repositories
{
    public class TasksRepository : ITasksRepository
    {
        private readonly TaskAPIDbContext _context;

        public TasksRepository(TaskAPIDbContext context)
        {
            _context = context;
        }

        // Отримати всі завдання
        public async Task<List<TaskEntity>> GetAll()
        {
            return await _context.Tasks
                .Include(t => t.TaskCreatedBy)
                .Include(t => t.TaskAssignedTo)
                .Include(t => t.Category)
                .Include(t => t.Project)
                .Include(t => t.Tags)
                .Include(t => t.Notifications)
                .ToListAsync();
        }

        // Отримати завдання за ID
        public async Task<TaskEntity?> GetById(Guid id)
        {
            return await _context.Tasks
                .Include(t => t.TaskCreatedBy)
                .Include(t => t.TaskAssignedTo)
                .Include(t => t.Category)
                .Include(t => t.Project)
                .Include(t => t.Tags)
                .Include(t => t.Notifications)
                .FirstOrDefaultAsync(t => t.Id == id) ?? throw new Exception("Task not found"); ;
        }
        public async Task Add(TaskEntity task)
        {
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
        }

        // Оновити завдання
        public async Task Update(TaskEntity task)
        {
            var existingTask = await _context.Tasks
                .Include(t => t.Tags)
                .Include(t => t.Notifications)
                .FirstOrDefaultAsync(t => t.Id == task.Id);

            if (existingTask == null)
            {
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
        }

        public async Task UpdateStatusById(TaskEntity task)
        {
            var existingTask = await _context.Tasks
                .Include(t => t.Tags)
                .Include(t => t.Notifications)
                .FirstOrDefaultAsync(t => t.Id == task.Id);

            if (existingTask == null)
            {
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
        }

        // Видалити завдання
        public async Task Delete(Guid id)
        {
            // Знаходимо завдання разом із пов’язаними повідомленнями
            var task = await _context.Tasks
                .Include(t => t.Notifications) // Додаємо пов’язані повідомлення
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task != null)
            {
                // Видаляємо всі пов’язані повідомлення
                _context.Notifications.RemoveRange(task.Notifications);

                // Видаляємо саме завдання
                _context.Tasks.Remove(task);

                // Зберігаємо зміни в базі даних
                await _context.SaveChangesAsync();
            }
        }
        //public async Task Delete(Guid id)
        //{
        //    var task = await _context.Tasks.FindAsync(id);
        //    if (task != null)
        //    {
        //        _context.Tasks.Remove(task);
        //        await _context.SaveChangesAsync();
        //    }
        //}

        // Отримати відфільтровані завдання
        public async Task<List<TaskEntity>> GetFilteredTasks(Guid userId, string? search, int? status, int? priority, DateTime? deadline, string? project, string? tag)
        {
            var query = _context.Tasks
                .Include(t => t.TaskCreatedBy)
                .Include(t => t.TaskAssignedTo)
                .Include(t => t.Category)
                .Include(t => t.Project)
                .Include(t => t.Tags)
                .Include(t => t.Notifications)
                .Where(t => t.TaskCreatedById == userId || t.TaskAssignedToId == userId) // Додаємо фільтр по користувачу
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

            return await query.ToListAsync();
        }

        public async Task UpdateStatus(Guid id, TaskManagerAPIPractice.Core.Model.TaskStatus status)
        {
            var existingTask = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (existingTask == null)
            {
                throw new Exception("Task not found");
            }

            existingTask.Status = status;
            _context.Entry(existingTask).Property(x => x.Status).IsModified = true;

            await _context.SaveChangesAsync();
        }

        public async Task UpdatePriority(Guid id, TaskManagerAPIPractice.Core.Model.TaskPriority priority)
        {
            var existingTask = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (existingTask == null)
            {
                throw new Exception("Task not found");
            }

            existingTask.Priority = priority;
            _context.Entry(existingTask).Property(x => x.Priority).IsModified = true;

            await _context.SaveChangesAsync();
        }

        public async Task<List<TaskEntity>> GetAllByUser(Guid userId)
        {
            var taskEntities = await _context.Tasks
                .Where(t => t.TaskCreatedById == userId || t.TaskAssignedToId == userId)
                .Include(t => t.TaskCreatedBy)
                .Include(t => t.TaskAssignedTo)
                .Include(t => t.Category)
                .Include(t => t.Project)
                .Include(t => t.Tags)
                .Include(t => t.Notifications)
                .AsNoTracking()
                .ToListAsync();

            return taskEntities;
        }
    }
}