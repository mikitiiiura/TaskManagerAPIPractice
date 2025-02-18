using Azure.Core;
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
    public class UpdateProjectHandler : IRequestHandler<UpdateProjectCommand, ProjectResponse>
    {
        private readonly IProjectsRepository _projectsRepository;
        private readonly TaskAPIDbContext _dbContext;
        private readonly ILogger<UpdateProjectHandler> _logger;

        public UpdateProjectHandler(IProjectsRepository projectsRepository, TaskAPIDbContext dbContext, ILogger<UpdateProjectHandler> logger)
        {
            _projectsRepository = projectsRepository;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<ProjectResponse> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling UpdateProjectCommand for Project ID: {ProjectId}", request.Id);

            var existingProject = await _projectsRepository.GetById(request.Id);
            if (existingProject == null)
            {
                _logger.LogWarning("Project with ID {ProjectId} not found", request.Id);
                throw new KeyNotFoundException("Project not found");
            }

            existingProject.Title = request.Title;
            existingProject.Description = request.Description;
            existingProject.EndDate = request.EndDate;
            existingProject.Status = (ProjectStatus)request.Status;
            existingProject.TeamId = request.TeamId;
            existingProject.ProjectCreatedById = request.projectCreatedById;

            _logger.LogInformation("Updating project: {ProjectId}", existingProject.Id);
            await _projectsRepository.Update(existingProject);
            await _dbContext.SaveChangesAsync();

            var notifications = new List<NotificationEntity>
            {
                new NotificationEntity
                {
                    Id = Guid.NewGuid(),
                    Message = $"Project '{existingProject.Title}' updated.",
                    CreatedAt = DateTime.UtcNow,
                    UserId = existingProject.ProjectCreatedById.Value,
                    TaskId = existingProject.Id
                }
            };

            _logger.LogInformation("Creating notification for project update: {ProjectId}", existingProject.Id);
            await _dbContext.Notifications.AddRangeAsync(notifications);

            _logger.LogInformation("Successfully updated project: {ProjectId}", existingProject.Id);
            return new ProjectResponse(existingProject);
        }
    }
}
