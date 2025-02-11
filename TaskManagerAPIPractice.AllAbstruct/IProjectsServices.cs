using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.DataAccess.Repositories
{
    public interface IProjectsServices
    {
        Task Add(ProjectEntity project);
        Task Delete(Guid id);
        Task<List<ProjectEntity>> GetAll();
        Task<ProjectEntity?> GetById(Guid id);
        Task<List<ProjectEntity>> GetFilteredProject(string? search, int? status, string? team);
        Task Update(ProjectEntity project);
    }
}