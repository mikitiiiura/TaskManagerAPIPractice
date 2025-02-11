
using TaskManagerAPIPractice.DataAccess.abstruction;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.Application.Services
{
    public class CategoryServices : ICategoryServices
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryServices(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<CategoryEntity>> GetAll()
        {
            return await _categoryRepository.GetAll();
        }

        public async Task<CategoryEntity?> GetById(Guid id)
        {
            return await _categoryRepository.GetById(id);
        }

        public async Task Add(CategoryEntity category)
        {
            await _categoryRepository.Add(category);
        }

        public async Task Update(CategoryEntity category)
        {
            await _categoryRepository.Update(category);
        }

        public async Task Delete(Guid id)
        {
            await _categoryRepository.Delete(id);
        }
    }
}
