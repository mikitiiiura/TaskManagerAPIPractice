using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagerAPIPractice.DataAccess.abstruction;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.DataAccess.Repositories
{
    public class NotificationsRepository : INotificationsRepository
    {
        private readonly TaskAPIDbContext _context;
        private readonly ILogger<NotificationsRepository> _logger;

        public NotificationsRepository(TaskAPIDbContext context, ILogger<NotificationsRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<NotificationEntity>> GetAll()
        {
            _logger.LogInformation("Fetching all notifications.");
            return await _context.Notifications.Include(n => n.User).Include(n => n.Task).AsNoTracking().ToListAsync();
        }

        public async Task<NotificationEntity?> GetById(Guid id)
        {
            _logger.LogInformation($"Fetching notification with ID: {id}");
            return await _context.Notifications
                .Include(n => n.User)
                .Include(n => n.Task)
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<List<NotificationEntity>> GetByIdUser(Guid userId)
        {
            _logger.LogInformation($"Fetching notifications for user ID: {userId}");
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .Include(n => n.User)
                .Include(n => n.Task)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task Add(NotificationEntity notification)
        {
            _logger.LogInformation($"Adding new notification for user ID: {notification.UserId}");
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task Update(NotificationEntity notification)
        {
            _logger.LogInformation($"Updating notification ID: {notification.Id}");
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            _logger.LogInformation($"Deleting notification ID: {id}");
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
            }
        }
    }
}
