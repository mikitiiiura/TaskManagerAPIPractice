
using TaskManagerAPIPractice.Core.Model;
using TaskManagerAPIPractice.DataAccess.abstruction;
using TaskManagerAPIPractice.DataAccess.ModulEntity;
using TaskManagerAPIPractice.DataAccess.Repositories;
using TaskManagerAPIPractice.Persistence;

namespace TaskManagerAPIPractice.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUsersRepository _usersRepository;
        private readonly IJwtProvider _jwtProvider;

        public UserService(IPasswordHasher passwordHasher, IUsersRepository usersRepository, IJwtProvider jwtProvider)
        {
            _passwordHasher = passwordHasher;
            _usersRepository = usersRepository;
            _jwtProvider = jwtProvider;
        }
        public async Task Register(string username, string email, string password)
        {
            var existingUser = await _usersRepository.GetByEmailForLogin(email);
            if (existingUser != null)
            {
                throw new Exception("Email is already in use.");
            }

            var hashedPassword = _passwordHasher.Generate(password);

            var user = User.Create(Guid.NewGuid(), username, email, hashedPassword, DateTime.UtcNow).User;

            await _usersRepository.AddForLogin(user);
        }

        //public async Task<string> Login(string email, string password)
        //{
        //    var user = await _usersRepository.GetByEmailForLogin(email);

        //    var result = _passwordHasher.Verify(password, user.PasswordHash);

        //    if (result == false)
        //    {
        //        throw new Exception("Faild to login");
        //    }

        //    var token = _jwtProvider.GenerateToken(user);
        //    return token;
        //}

        public async Task<LoginResult> Login(string email, string password)
        {
            var user = await _usersRepository.GetByEmailForLogin(email);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var result = _passwordHasher.Verify(password, user.PasswordHash);
            if (!result)
            {
                throw new Exception("Failed to login");
            }

            var token = _jwtProvider.GenerateToken(user);

            return new LoginResult
            {
                Token = token,
                UserId = user.Id // Припустимо, що у користувача є поле Id
            };
        }




        ///////////////////////////////////////////////////////////////////////////////////////////////////

        public async Task<List<UserEntity>> GetAllUsers()
        {
            return await _usersRepository.GetAll();
        }

        public async Task<UserEntity?> GetUserById(Guid id)
        {
            return await _usersRepository.GetById(id);
        }

        public async Task UpdateUser(UserEntity user)
        {
            await _usersRepository.Update(user);
        }

        public async Task<bool> DeleteUser(Guid id)
        {
            return await _usersRepository.Delete(id);
        }

    }

}
