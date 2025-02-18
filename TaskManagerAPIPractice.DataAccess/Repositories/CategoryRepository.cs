using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagerAPIPractice.DataAccess.abstruction;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.DataAccess.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly TaskAPIDbContext _context;
        private readonly ILogger<CategoryRepository> _logger;

        public CategoryRepository(TaskAPIDbContext context, ILogger<CategoryRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<CategoryEntity>> GetAll()
        {
            _logger.LogInformation("Fetching all categories");
            return await _context.CategoryEntities
                .Include(c => c.CategoryCreatedBy)
                .Include(c => c.Tasks)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CategoryEntity?> GetById(Guid id)
        {
            _logger.LogInformation("Fetching category with ID: {Id}", id);
            return await _context.CategoryEntities
                .Include(c => c.CategoryCreatedBy)
                .Include(c => c.Tasks)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task Add(CategoryEntity category)
        {
            _logger.LogInformation("Adding a new category with ID: {Id}", category.Id);
            await _context.CategoryEntities.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task Update(CategoryEntity category)
        {
            _logger.LogInformation("Updating category with ID: {Id}", category.Id);
            var existingCategory = await _context.CategoryEntities
                .Include(c => c.CategoryCreatedBy)
                .FirstOrDefaultAsync(c => c.Id == category.Id);

            if (existingCategory == null)
            {
                _logger.LogWarning("Category not found with ID: {Id}", category.Id);
                throw new Exception("Category not found");
            }

            existingCategory.Title = category.Title;
            existingCategory.CategoryCreatedById = category.CategoryCreatedById;

            _context.CategoryEntities.Update(existingCategory);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            _logger.LogInformation("Deleting category with ID: {Id}", id);
            var category = await _context.CategoryEntities.FindAsync(id);
            if (category != null)
            {
                _context.CategoryEntities.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}
