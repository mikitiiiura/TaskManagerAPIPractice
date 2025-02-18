using MediatR;
using Microsoft.Extensions.Logging;
using TaskManagerAPIPractice.Application.Contracts.Command;
using TaskManagerAPIPractice.Contracts.Response;
using TaskManagerAPIPractice.Core.Model;
using TaskManagerAPIPractice.DataAccess;
using TaskManagerAPIPractice.DataAccess.abstruction;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.Application.Handlers.ProjectHandler
{
    public class CreateProjectHandler : IRequestHandler<CreateProjectCommand, ProjectResponse>
    {
        private readonly IProjectsRepository _projectsRepository;
        private readonly TaskAPIDbContext _dbContext;
        private readonly ILogger<CreateProjectHandler> _logger;

        public CreateProjectHandler(IProjectsRepository projectsRepository, TaskAPIDbContext dbContext, ILogger<CreateProjectHandler> logger)
        {
            _projectsRepository = projectsRepository;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<ProjectResponse> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting project creation: {ProjectTitle}", request.Title);

            var project = new ProjectEntity
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                StartDate = DateTime.UtcNow,
                EndDate = request.EndDate,
                Status = (ProjectStatus)request.Status,
                TeamId = request.TeamId,
                ProjectCreatedById = request.projectCreatedById
            };

            await _projectsRepository.Add(project);
            _logger.LogInformation("Project {ProjectTitle} (ID: {ProjectId}) created successfully.", project.Title, project.Id);

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Project {ProjectTitle} saved to database.", project.Title);

            // Додаємо сповіщення після створення проєкту
            var notifications = new List<NotificationEntity>
            {
                new NotificationEntity
                {
                    Id = Guid.NewGuid(),
                    Message = $"Project '{project.Title}' created.",
                    CreatedAt = DateTime.UtcNow,
                    UserId = project.ProjectCreatedById.Value,
                    TaskId = project.Id
                }
            };

            await _dbContext.Notifications.AddRangeAsync(notifications);
            _logger.LogInformation("Notification for project {ProjectTitle} created.", project.Title);

            return new ProjectResponse(project);
        }
    }
}
