using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.DataAccess.abstruction
{
    public interface ITagRepository
    {
        Task AddAsync(TagEntity tag);
        Task DeleteAsync(Guid id);
        Task<List<TagEntity>> GetAllAsync();
        Task<TagEntity?> GetByIdAsync(Guid id);
        Task UpdateAsync(TagEntity tag);
    }
}