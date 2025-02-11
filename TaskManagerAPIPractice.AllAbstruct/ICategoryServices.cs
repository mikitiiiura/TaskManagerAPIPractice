using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.Application.Services
{
    public interface ICategoryServices
    {
        Task Add(CategoryEntity category);
        Task Delete(Guid id);
        Task<List<CategoryEntity>> GetAll();
        Task<CategoryEntity?> GetById(Guid id);
        Task Update(CategoryEntity category);
    }
}