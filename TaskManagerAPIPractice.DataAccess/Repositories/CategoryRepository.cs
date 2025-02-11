
using Microsoft.EntityFrameworkCore;
using TaskManagerAPIPractice.DataAccess.abstruction;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.DataAccess.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly TaskAPIDbContext _context;

        public CategoryRepository(TaskAPIDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryEntity>> GetAll()
        {
            return await _context.CategoryEntities
                .Include(c => c.CategoryCreatedBy)
                .Include(c => c.Tasks)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CategoryEntity?> GetById(Guid id)
        {
            return await _context.CategoryEntities
                .Include(c => c.CategoryCreatedBy)
                .Include(c => c.Tasks)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task Add(CategoryEntity category)
        {
            await _context.CategoryEntities.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task Update(CategoryEntity category)
        {
            var existingCategory = await _context.CategoryEntities
                .Include(c => c.CategoryCreatedBy)
                .FirstOrDefaultAsync(c => c.Id == category.Id);

            if (existingCategory == null)
            {
                throw new Exception("Category not found");
            }

            existingCategory.Title = category.Title;
            existingCategory.CategoryCreatedById = category.CategoryCreatedById;

            _context.CategoryEntities.Update(existingCategory);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var category = await _context.CategoryEntities.FindAsync(id);
            if (category != null)
            {
                _context.CategoryEntities.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}
