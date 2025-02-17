
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPIPractice.Application.Contracts.Command;
using TaskManagerAPIPractice.Application.Services;
using TaskManagerAPIPractice.Contracts;
using TaskManagerAPIPractice.Contracts.Request;
using TaskManagerAPIPractice.Contracts.Response;
using TaskManagerAPIPractice.Core.Model;
using TaskManagerAPIPractice.DataAccess;
using TaskManagerAPIPractice.DataAccess.abstruct;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.Application.Handlers.TasksHandler
{
    public class CreateTasksHandler : IRequestHandler<CreateTaskCommand, TaskResponse>
    {
        private readonly ITasksRepository _tasksRepository;
        private readonly TaskAPIDbContext _dbContext;


        public CreateTasksHandler(ITasksRepository tasksRepository, TaskAPIDbContext dbContext)
        {
            _tasksRepository = tasksRepository;
            _dbContext = dbContext;
        }

        public async Task<TaskResponse> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            //var tasks = await _tasksRepository.GetAllByUser(request.UserId);
            //return tasks.Select(task => new TaskResponse(task)).ToList();
            var existingTags = _dbContext.Tags.Where(tag => request.Tags.Contains(tag.Id)).ToList();

            var taskEntity = new TaskEntity
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                Status = (Core.Model.TaskStatus)request.Status,
                Priority = (TaskPriority)request.Priority,
                DeadLine = request.DeadLine,
                CreatedAt = DateTime.UtcNow,
                //TaskCreatedById = Guid.Parse(userId),
                TaskCreatedById = request.TaskCreatedById,
                TaskAssignedToId = request.TaskAssignedToId,
                CategoryId = request.CategoryId,
                ProjectId = request.ProjectId,
                TeamId = request.TeamId,
                Tags = request.Tags?
                    .Select(tagId => existingTags.FirstOrDefault(t => t.Id == tagId) ?? new TagEntity { Id = tagId })
                    .ToList() ?? new List<TagEntity>()
            };

            await _tasksRepository.Add(taskEntity);
            await _dbContext.SaveChangesAsync(); // Примушуємо EF оновити `task.Id`

            // Додаємо сповіщення після створення задачі
            var notifications = new List<NotificationEntity>
            {
                new NotificationEntity
                {
                    Id = Guid.NewGuid(),
                    Message = $"Завдання '{taskEntity.Title}' створено.",
                    CreatedAt = DateTime.UtcNow,
                    UserId = taskEntity.TaskCreatedById,
                    TaskId = taskEntity.Id
                }
            };

            if (taskEntity.TaskAssignedToId != null)
            {
                notifications.Add(new NotificationEntity
                {
                    Id = Guid.NewGuid(),
                    Message = $"Вам призначено нове завдання: '{taskEntity.Title}'.",
                    CreatedAt = DateTime.UtcNow,
                    UserId = taskEntity.TaskAssignedToId.Value,
                    TaskId = taskEntity.Id
                });
            }

            await _dbContext.Notifications.AddRangeAsync(notifications);

            return new TaskResponse(taskEntity);
        }
    }
}
