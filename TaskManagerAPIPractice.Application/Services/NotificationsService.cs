
using TaskManagerAPIPractice.DataAccess.abstruction;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.Application.Services
{
    public class NotificationsService : INotificationsService
    {
        private readonly INotificationsRepository _notificationsRepository;

        public NotificationsService(INotificationsRepository notificationsRepository)
        {
            _notificationsRepository = notificationsRepository;
        }

        public async Task<List<NotificationEntity>> GetAll()
        {
            return await _notificationsRepository.GetAll();
        }

        public async Task<NotificationEntity?> GetById(Guid id)
        {
            return await _notificationsRepository.GetById(id);
        }

        public async Task Add(NotificationEntity notification)
        {
            await _notificationsRepository.Add(notification);
        }

        public async Task Update(NotificationEntity notification)
        {
            await _notificationsRepository.Update(notification);
        }

        public async Task Delete(Guid id)
        {
            await _notificationsRepository.Delete(id);
        }
    }
}
