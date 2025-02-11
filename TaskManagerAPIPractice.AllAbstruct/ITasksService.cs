using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.Application.Services
{
    public interface ITasksService
    {
        Task Add(TaskEntity task);
        Task Delete(Guid id);
        Task<List<TaskEntity>> GetAll();
        Task<TaskEntity?> GetById(Guid id);
        Task<List<TaskEntity>> GetFilteredTasks(string? search, int? status, int? priority, DateTime? deadline, string? project, string? tag);
        Task Update(TaskEntity task);
    }
}