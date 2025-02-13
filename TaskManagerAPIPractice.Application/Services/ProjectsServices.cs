using Microsoft.EntityFrameworkCore;
using TaskManagerAPIPractice.Core.Model;
using TaskManagerAPIPractice.DataAccess.abstruct;
using TaskManagerAPIPractice.DataAccess.abstruction;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.DataAccess.Repositories
{
    public class ProjectsServices : IProjectsServices
    {
        private readonly IProjectsRepository _projectsRepository;

        public ProjectsServices(IProjectsRepository projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }
        
        // Отримати всі проекти
        public async Task<List<ProjectEntity>> GetAll()
        {
            return await _projectsRepository.GetAll();
        }

        // Отримати проект за ID
        public async Task<ProjectEntity?> GetById(Guid id)
        {
            return await _projectsRepository.GetById(id);
        }

        // Додати новий проект
        public async Task Add(ProjectEntity project)
        {

            await _projectsRepository.Add(project);

        }

        // Оновити проект
        public async Task Update(ProjectEntity project)
        {
            await _projectsRepository.Update(project);
        }

        // Видалити проект
        public async Task Delete(Guid id)
        {
            await _projectsRepository.Delete(id);
        }

        // Отримати відфільтровані проекти
        public async Task<List<ProjectEntity>> GetFilteredProject(Guid userId, string? search, int? status, string? team)
        {
            return await _projectsRepository.GetFiltered(userId, search, status, team);
        }

        public async Task UpdateStatus(Guid id, TaskManagerAPIPractice.Core.Model.ProjectStatus status)
        {
            await _projectsRepository.UpdateStatus(id, status);
        }

        public async Task<List<ProjectEntity>> GetAllByUser(Guid userId)
        {
            return await _projectsRepository.GetAllByUser(userId);
        }
    }
}