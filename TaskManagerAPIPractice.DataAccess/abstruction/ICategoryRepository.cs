using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.DataAccess.abstruction
{
    public interface ICategoryRepository
    {
        Task Add(CategoryEntity category);
        Task Delete(Guid id);
        Task<List<CategoryEntity>> GetAll();
        Task<CategoryEntity?> GetById(Guid id);
        Task Update(CategoryEntity category);
    }
}