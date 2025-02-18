using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<CreateTasksHandler> _logger;

        public CreateTasksHandler(ITasksRepository tasksRepository, TaskAPIDbContext dbContext, ILogger<CreateTasksHandler> logger)
        {
            _tasksRepository = tasksRepository;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<TaskResponse> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating task: {Title}", request.Title);
            var existingTags = _dbContext.Tags.Where(tag => request.Tags.Contains(tag.Id)).ToList();

            var taskEntity = new TaskEntity
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                Status = (Core.Model.TaskStatus)request.Status,
                Priority = (TaskPriority)request.Priority,
                DeadLine = request.DeadLine,
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
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Task created with ID: {TaskId}", taskEntity.Id);

            var notifications = new List<NotificationEntity>
            {
                new NotificationEntity
                {
                    Id = Guid.NewGuid(),
                    Message = $"Task '{taskEntity.Title}' created.",
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
                    Message = $"You are assigned a new task: '{taskEntity.Title}'.",
                    CreatedAt = DateTime.UtcNow,
                    UserId = taskEntity.TaskAssignedToId.Value,
                    TaskId = taskEntity.Id
                });
            }

            await _dbContext.Notifications.AddRangeAsync(notifications);
            _logger.LogInformation("Notifications created for task: {TaskId}", taskEntity.Id);

            return new TaskResponse(taskEntity);
        }
    }
}
