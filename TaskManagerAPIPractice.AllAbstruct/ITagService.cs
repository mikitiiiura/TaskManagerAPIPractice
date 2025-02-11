using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.Application.Services
{
    public interface ITagService
    {
        Task AddAsync(TagEntity tag);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<TagEntity>> GetAllAsync();
        Task<TagEntity?> GetByIdAsync(Guid id);
        Task UpdateAsync(TagEntity tag);
    }
}