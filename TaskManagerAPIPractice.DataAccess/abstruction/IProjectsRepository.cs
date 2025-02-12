using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.DataAccess.abstruction
{
    public interface IProjectsRepository
    {
        Task Add(ProjectEntity project);
        Task Delete(Guid id);
        Task<List<ProjectEntity>> GetAll();
        Task<ProjectEntity> GetById(Guid id);
        Task<List<ProjectEntity>> GetFiltered(string? search, int? status, string? team);
        Task Update(ProjectEntity project);
        Task UpdateStatus(Guid id, TaskManagerAPIPractice.Core.Model.ProjectStatus status);
        Task<List<ProjectEntity>> GetAllByUser(Guid userId);
    }
}