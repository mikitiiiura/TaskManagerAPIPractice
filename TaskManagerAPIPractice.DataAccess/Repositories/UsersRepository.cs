using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagerAPIPractice.Core.Model;
using TaskManagerAPIPractice.DataAccess.abstruction;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.DataAccess.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly TaskAPIDbContext _context;
        private readonly ILogger<UsersRepository> _logger;

        public UsersRepository(TaskAPIDbContext context, ILogger<UsersRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<UserEntity>> GetAll()
        {
            _logger.LogInformation("Fetching all users");
            return await _context.Users
                .Include(u => u.Team)
                .Include(u => u.Tags)
                .Include(u => u.CreatedTasks)
                .ToListAsync();
        }

        public async Task<UserEntity?> GetById(Guid id)
        {
            _logger.LogInformation("Fetching user with ID: {UserId}", id);
            return await _context.Users
                .Include(u => u.Team)
                .Include(u => u.Tags)
                .Include(u => u.CreatedTasks)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task AddForLogin(User user)
        {
            _logger.LogInformation("Adding user for login: {Email}", user.Email);
            var userEntity = new UserEntity
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                CreatedAt = user.CreatedAt
            };

            await _context.Users.AddAsync(userEntity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(UserEntity user)
        {
            _logger.LogInformation("Updating user with ID: {UserId}", user.Id);
            var existingUser = await _context.Users
                .Include(u => u.Tags)
                .Include(u => u.Team)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            if (existingUser == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", user.Id);
                throw new Exception("User not found");
            }

            existingUser.FullName = user.FullName;
            existingUser.Email = user.Email;
            existingUser.CreatedAt = user.CreatedAt;
            existingUser.TeamId = user.TeamId;
            existingUser.Tags = user.Tags;

            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> Delete(Guid id)
        {
            _logger.LogInformation("Deleting user with ID: {UserId}", id);
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", id);
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User?> GetByEmailForLogin(string email)
        {
            _logger.LogInformation("Fetching user by email for login: {Email}", email);
            var userEntity = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);

            if (userEntity == null)
            {
                _logger.LogWarning("User with email {Email} not found", email);
                return null;
            }

            return new User(userEntity.Id, userEntity.FullName, userEntity.Email,
                userEntity.PasswordHash, userEntity.CreatedAt);
        }
    }
}
