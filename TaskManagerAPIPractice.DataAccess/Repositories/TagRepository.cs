using Microsoft.EntityFrameworkCore;
using TaskManagerAPIPractice.DataAccess.abstruction;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.DataAccess.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly TaskAPIDbContext _context;
        public TagRepository(TaskAPIDbContext context) => _context = context;

        public async Task<List<TagEntity>> GetAllAsync() => await _context.Tags.Include(t => t.TagCreatedBy).Include(t => t.Tasks).ToListAsync();
        public async Task<TagEntity?> GetByIdAsync(Guid id) => await _context.Tags.Include(t => t.TagCreatedBy).Include(t => t.Tasks).FirstOrDefaultAsync(t => t.TagCreatedById == id);
        public async Task AddAsync(TagEntity tag) { _context.Tags.Add(tag); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(TagEntity tag) { _context.Tags.Update(tag); await _context.SaveChangesAsync(); }


        // Видалити проект
        public async Task DeleteAsync(Guid id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag != null)
            {
                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync();
            }
        }
    }
}
