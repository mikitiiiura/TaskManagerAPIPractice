using Microsoft.EntityFrameworkCore;
using TaskManagerAPIPractice.DataAccess.abstruction;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.DataAccess.Repositories
{
    public class ProjectsRepository : IProjectsRepository
    {
        private readonly TaskAPIDbContext _context;

        public ProjectsRepository(TaskAPIDbContext context)
        {
            _context = context;
        }

        // Отримати всі проекти
        public async Task<List<ProjectEntity>> GetAll()
        {
            var projectEntities = await _context.Projects
                .Include(p => p.Team)
                .Include(p => p.ProjectCreatedBy)
                .Include(p => p.Tasks) //небуло
                .AsNoTracking()
                .ToListAsync();

            //return projectEntities.Select(p => MapToProject(p)).ToList();
            return projectEntities.ToList();
        }

        // Отримати проект за ID
        public async Task<ProjectEntity> GetById(Guid id)
        {
            var projectEntity = await _context.Projects
                .Include(p => p.Team)
                .Include(p => p.ProjectCreatedBy)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id) ?? throw new Exception("Project not found");

            return projectEntity;
        }

        // Додати новий проект
        public async Task Add(ProjectEntity project)
        {
            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();
        }

        // Оновити проект
        public async Task Update(ProjectEntity project)
        {
            var existingProject = await _context.Projects
                .Include(p => p.Team)
                .Include(p => p.ProjectCreatedBy)
                .FirstOrDefaultAsync(p => p.Id == project.Id);

            if (existingProject == null)
            {
                throw new Exception("Project not found");
            }

            // Оновлення основних полів
            existingProject.Title = project.Title;
            existingProject.Description = project.Description;
            existingProject.StartDate = project.StartDate;
            existingProject.EndDate = project.EndDate;
            existingProject.Status = project.Status;

            // Оновлення зовнішніх ключів
            existingProject.TeamId = project.TeamId;
            existingProject.ProjectCreatedById = project.ProjectCreatedById;

            _context.Projects.Update(existingProject);
            await _context.SaveChangesAsync();
        }

        // Видалити проект
        public async Task Delete(Guid id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }
        }

        // Отримати відфільтровані проекти
        public async Task<List<ProjectEntity>> GetFiltered(string? search, int? status, string? team)
        {
            var query = _context.Projects
                .Include(p => p.Team)
                .Include(p => p.ProjectCreatedBy)
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
            //return projectEntities.Select(p => MapToProject(p)).ToList();
        }

        public async Task UpdateStatus(Guid id, TaskManagerAPIPractice.Core.Model.ProjectStatus status)
        {
            var existingProject = await _context.Projects.FirstOrDefaultAsync(t => t.Id == id);
            if (existingProject == null)
            {
                throw new Exception("Project not found");
            }

            existingProject.Status = status;
            _context.Entry(existingProject).Property(x => x.Status).IsModified = true;

            await _context.SaveChangesAsync();
        }

        public async Task<List<ProjectEntity>> GetAllByUser(Guid userId)
        {
            var projectEntities = await _context.Projects
                .Where(p => p.ProjectCreatedById == userId)
                .Include(p => p.Team)
                .Include(p => p.ProjectCreatedBy)
                .Include(p => p.Tasks) //небуло
                .AsNoTracking()
                .ToListAsync();

            return projectEntities.ToList();

            //return await _context.Projects
            //    .Where(p => p.ProjectCreatedById == userId)
            //    .ToListAsync();
        }

    }
}