
using MediatR;
using TaskManagerAPIPractice.Application.Contracts.Command;
using TaskManagerAPIPractice.Contracts.Response;
using TaskManagerAPIPractice.DataAccess.abstruct;
using TaskManagerAPIPractice.DataAccess;
using TaskManagerAPIPractice.DataAccess.abstruction;
using TaskManagerAPIPractice.DataAccess.Repositories;
using TaskManagerAPIPractice.Contracts.Request;
using TaskManagerAPIPractice.DataAccess.ModulEntity;
using Microsoft.EntityFrameworkCore;

namespace TaskManagerAPIPractice.Application.Handlers.TasksHandler
{
    public class UpdateTaskHandler : IRequestHandler<UpdateTaskCommand, TaskResponse>
    {
        private readonly ITasksRepository _tasksRepository;
        private readonly ITagRepository _tagRepository;
        private readonly TaskAPIDbContext _dbContext;

        public UpdateTaskHandler(ITasksRepository tasksRepository, ITagRepository tagRepository, TaskAPIDbContext dbContext)
        {
            _tasksRepository = tasksRepository;
            _tagRepository = tagRepository;
            _dbContext = dbContext;
        }

        public async Task<TaskResponse> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            var existingTask = await _tasksRepository.GetById(request.Id);
            if (existingTask == null)
                throw new KeyNotFoundException("Task not found");


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
            await _dbContext.SaveChangesAsync(); // Примушуємо EF оновити 

            // Додаємо сповіщення після створення задачі
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

            return new TaskResponse(existingTask);

        }
    }
}
