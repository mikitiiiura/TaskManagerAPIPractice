using TaskManagerAPIPractice.Core.Model;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.DataAccess.abstruction
{
    public interface IUsersRepository
    {
        Task<List<UserEntity>> GetAll();
        Task<UserEntity?> GetById(Guid id);
        Task Update(UserEntity user);
        Task<bool> Delete(Guid id);
        Task<User?> GetByEmailForLogin(string email);

        Task AddForLogin(User user);
        //Task<Guid> Update(Guid id, string fullName, string email, string passwordHash, DateTime createdAt);
    }
}