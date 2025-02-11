using Microsoft.EntityFrameworkCore;
using System.Threading;
using TaskManagerAPIPractice.Core.Model;
using TaskManagerAPIPractice.DataAccess.abstruction;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.DataAccess.Repositories
{
    public class NotificationsRepository : INotificationsRepository
    {
        private readonly TaskAPIDbContext _context;

        public NotificationsRepository(TaskAPIDbContext context)
        {
            _context = context;
        }

        public async Task<List<NotificationEntity>> GetAll()
        {
            return await _context.Notifications.Include(n => n.User).Include(n => n.Task).AsNoTracking().ToListAsync();
        }

        public async Task<NotificationEntity?> GetById(Guid id)
        {
            return await _context.Notifications.Include(n => n.User).Include(n => n.Task).AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task Add(NotificationEntity notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task Update(NotificationEntity notification)
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
            }
        }
    }
}
