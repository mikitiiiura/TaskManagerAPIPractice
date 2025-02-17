using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.Application.Services
{
    public interface INotificationsService
    {
        Task Add(NotificationEntity notification);
        Task Delete(Guid id);
        Task<List<NotificationEntity>> GetAll();
        Task<NotificationEntity?> GetById(Guid id);
        Task Update(NotificationEntity notification);

        Task<List<NotificationEntity>> GetByIdUser(Guid userId);
    }
}