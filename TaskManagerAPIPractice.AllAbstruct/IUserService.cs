
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.Application.Services
{
    public interface IUserService
    {
        //Task<string> Login(string email, string password);
        Task<LoginResult> Login(string email, string password);
        Task Register(string username, string email, string password);

        Task<List<UserEntity>> GetAllUsers();

        Task<UserEntity?> GetUserById(Guid id);

        Task UpdateUser(UserEntity user);
        Task<bool> DeleteUser(Guid id);
    }
}