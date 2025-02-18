using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagerAPIPractice.DataAccess.abstruction;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.DataAccess.Repositories
{
    public class ProjectsRepository : IProjectsRepository
    {
        private readonly TaskAPIDbContext _context;
        private readonly ILogger<ProjectsRepository> _logger;

        public ProjectsRepository(TaskAPIDbContext context, ILogger<ProjectsRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<ProjectEntity>> GetAll()
        {
            _logger.LogInformation("Fetching all projects");
            return await _context.Projects
                .Include(p => p.Team)
                .Include(p => p.ProjectCreatedBy)
                .Include(p => p.Tasks)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ProjectEntity> GetById(Guid id)
        {
            _logger.LogInformation("Fetching project with ID: {ProjectId}", id);
            var projectEntity = await _context.Projects
                .Include(p => p.Team)
                .Include(p => p.ProjectCreatedBy)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (projectEntity == null)
            {
                _logger.LogWarning("Project with ID {ProjectId} not found", id);
                throw new Exception("Project not found");
            }
            return projectEntity;
        }

        public async Task Add(ProjectEntity project)
        {
            _logger.LogInformation("Adding new project: {ProjectTitle}", project.Title);
            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();

            var notification = new NotificationEntity
            {
                Id = Guid.NewGuid(),
                Message = $"Project '{project.Title}' created.",
                CreatedAt = DateTime.UtcNow,
                UserId = project.ProjectCreatedById ?? throw new InvalidOperationException("ProjectCreatedById cannot be null."),
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task Update(ProjectEntity project)
        {
            _logger.LogInformation("Updating project with ID: {ProjectId}", project.Id);
            var existingProject = await _context.Projects.FirstOrDefaultAsync(p => p.Id == project.Id);

            if (existingProject == null)
            {
                _logger.LogWarning("Project with ID {ProjectId} not found", project.Id);
                throw new Exception("Project not found");
            }

            existingProject.Title = project.Title;
            existingProject.Description = project.Description;
            existingProject.StartDate = project.StartDate;
            existingProject.EndDate = project.EndDate;
            existingProject.Status = project.Status;
            existingProject.TeamId = project.TeamId;
            existingProject.ProjectCreatedById = project.ProjectCreatedById;

            _context.Projects.Update(existingProject);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            _logger.LogInformation("Deleting project with ID: {ProjectId}", id);
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                _logger.LogWarning("Project with ID {ProjectId} not found", id);
                return;
            }
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProjectEntity>> GetFiltered(Guid userId, string? search, int? status, string? team)
        {
            _logger.LogInformation("Fetching filtered projects for user {UserId}", userId);
            var query = _context.Projects
                .Include(p => p.Team)
                .Include(p => p.ProjectCreatedBy)
                .Include(p => p.Tasks)
                .Where(p => p.ProjectCreatedById == userId)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Title.Contains(search) || p.Description.Contains(search));
            }
            if (status.HasValue)
            {
                query = query.Where(p => (int)p.Status == status.Value);
            }
            if (!string.IsNullOrEmpty(team))
            {
                query = query.Where(p => p.Team != null && p.Team.Name == team);
            }
            return await query.ToListAsync();
        }

        public async Task UpdateStatus(Guid id, TaskManagerAPIPractice.Core.Model.ProjectStatus status)
        {
            _logger.LogInformation("Updating project status for ID: {ProjectId}", id);
            var existingProject = await _context.Projects.FirstOrDefaultAsync(t => t.Id == id);
            if (existingProject == null)
            {
                _logger.LogWarning("Project with ID {ProjectId} not found", id);
                throw new Exception("Project not found");
            }
            existingProject.Status = status;
            _context.Entry(existingProject).Property(x => x.Status).IsModified = true;
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProjectEntity>> GetAllByUser(Guid userId)
        {
            _logger.LogInformation("Fetching all projects for user ID: {UserId}", userId);
            return await _context.Projects
                .Where(p => p.ProjectCreatedById == userId)
                .Include(p => p.Team)
                .Include(p => p.ProjectCreatedBy)
                .Include(p => p.Tasks)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
