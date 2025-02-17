using MediatR;
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

        public CreateProjectHandler(IProjectsRepository projectsRepository, TaskAPIDbContext dbContext)
        {
            _projectsRepository = projectsRepository;
            _dbContext = dbContext;
        }

        public async Task<ProjectResponse> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
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
            await _dbContext.SaveChangesAsync();

            // Додаємо сповіщення після створення задачі
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

            return new ProjectResponse(project);
        }
    }
}
