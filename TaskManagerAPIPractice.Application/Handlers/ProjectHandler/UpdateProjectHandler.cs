
using Azure.Core;
using MediatR;
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

        public UpdateProjectHandler(IProjectsRepository projectsRepository, TaskAPIDbContext dbContext)
        {
            _projectsRepository = projectsRepository;
            _dbContext = dbContext;
        }

        public async Task<ProjectResponse> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var existingProject = await _projectsRepository.GetById(request.Id);
            if (existingProject == null) throw new KeyNotFoundException("Project not found"); ;

            existingProject.Title = request.Title;
            existingProject.Description = request.Description;
            existingProject.EndDate = request.EndDate;
            existingProject.Status = (ProjectStatus)request.Status;
            existingProject.TeamId = request.TeamId;
            existingProject.ProjectCreatedById = request.projectCreatedById;

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

            await _dbContext.Notifications.AddRangeAsync(notifications);

            return new ProjectResponse(existingProject);
        }
    }
}
