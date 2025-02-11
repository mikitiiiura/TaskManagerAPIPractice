using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.DataAccess.abstruction
{
    public interface INotificationsRepository
    {
        Task Add(NotificationEntity notification);
        Task Delete(Guid id);
        Task<List<NotificationEntity>> GetAll();
        Task<NotificationEntity?> GetById(Guid id);
        Task Update(NotificationEntity notification);
    }
}