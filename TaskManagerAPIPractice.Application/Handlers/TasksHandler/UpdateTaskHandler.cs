using MediatR;
using Microsoft.Extensions.Logging;
using TaskManagerAPIPractice.Application.Contracts.Command;
using TaskManagerAPIPractice.Contracts.Response;
using TaskManagerAPIPractice.DataAccess.abstruct;
using TaskManagerAPIPractice.DataAccess;
using TaskManagerAPIPractice.DataAccess.abstruction;
using TaskManagerAPIPractice.DataAccess.Repositories;
using TaskManagerAPIPractice.DataAccess.ModulEntity;
using Microsoft.EntityFrameworkCore;

namespace TaskManagerAPIPractice.Application.Handlers.TasksHandler
{
    public class UpdateTaskHandler : IRequestHandler<UpdateTaskCommand, TaskResponse>
    {
        private readonly ITasksRepository _tasksRepository;
        private readonly ITagRepository _tagRepository;
        private readonly TaskAPIDbContext _dbContext;
        private readonly ILogger<UpdateTaskHandler> _logger;

        public UpdateTaskHandler(ITasksRepository tasksRepository, ITagRepository tagRepository, TaskAPIDbContext dbContext, ILogger<UpdateTaskHandler> logger)
        {
            _tasksRepository = tasksRepository;
            _tagRepository = tagRepository;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<TaskResponse> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating task with ID: {TaskId}", request.Id);

            var existingTask = await _tasksRepository.GetById(request.Id);
            if (existingTask == null)
            {
                _logger.LogWarning("Task with ID {TaskId} not found", request.Id);
                throw new KeyNotFoundException("Task not found");
            }

            existingTask.Title = request.Title;
            existingTask.Description = request.Description;
            existingTask.Status = (Core.Model.TaskStatus)request.Status;
            existingTask.Priority = (Core.Model.TaskPriority)request.Priority;
            existingTask.DeadLine = request.DeadLine;
            existingTask.TaskAssignedToId = request.TaskAssignedToId;
            existingTask.CategoryId = request.CategoryId;
            existingTask.ProjectId = request.ProjectId;
            existingTask.TeamId = request.TeamId;
            existingTask.Tags = request.Tags?.Select(tagId => new TagEntity { Id = tagId }).ToList() ?? new List<TagEntity>();

            await _tasksRepository.Update(existingTask);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Task with ID {TaskId} successfully updated", request.Id);

            var notifications = new List<NotificationEntity>
            {
                new NotificationEntity
                {
                    Id = Guid.NewGuid(),
                    Message = $"Task '{existingTask.Title}' updated.",
                    CreatedAt = DateTime.UtcNow,
                    UserId = existingTask.TaskCreatedById,
                    TaskId = existingTask.Id
                }
            };

            if (existingTask.TaskAssignedToId != null)
            {
                notifications.Add(new NotificationEntity
                {
                    Id = Guid.NewGuid(),
                    Message = $"You are assigned a new task: '{existingTask.Title}'.",
                    CreatedAt = DateTime.UtcNow,
                    UserId = existingTask.TaskAssignedToId.Value,
                    TaskId = existingTask.Id
                });
            }

            await _dbContext.Notifications.AddRangeAsync(notifications);
            _logger.LogInformation("Notifications created for task ID {TaskId}", existingTask.Id);

            return new TaskResponse(existingTask);
        }
    }
}
