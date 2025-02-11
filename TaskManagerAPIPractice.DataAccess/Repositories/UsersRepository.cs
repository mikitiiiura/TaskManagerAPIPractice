using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TaskManagerAPIPractice.Core.Model;
using TaskManagerAPIPractice.DataAccess.abstruction;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.DataAccess.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly TaskAPIDbContext _context;

        public UsersRepository(TaskAPIDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserEntity>> GetAll()
        {
            return await _context.Users
                .Include(u => u.Team)
                .Include(u => u.Tags)
                .Include(u => u.CreatedTasks)
                .ToListAsync();
        }

        public async Task<UserEntity?> GetById(Guid id)
        {
            return await _context.Users
                .Include(u => u.Team)
                .Include(u => u.Tags)
                .Include(u => u.CreatedTasks)
                .FirstOrDefaultAsync(u => u.Id == id);
        }
        //public async Task<List<UserEntity>> GetAll()
        //{
        //    return await _context.Users
        //        .Include(u => u.AdministeredTeams)
        //        .Include(u => u.CreatedTasks)
        //        .Include(u => u.AssignedTasks)
        //        .Include(u => u.Projects)
        //        .Include(u => u.Categories)
        //        .Include(u => u.Tags)
        //        .Include(u => u.Notifications)
        //        .Include(u => u.Team)
        //        .ToListAsync();
        //}

        public async Task AddForLogin(User user)
        {
            var userEntity = new UserEntity //створюємо сутність щоб занести її в базу даних
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
            var existingUser = await _context.Users
                .Include(u => u.Tags)
                .Include(u => u.Team)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            if (existingUser == null)
            {
                throw new Exception("User not found");
            }

            // Оновлення основних полів
            existingUser.FullName = user.FullName;
            existingUser.Email = user.Email;
            existingUser.CreatedAt = user.CreatedAt;

            // Оновлення зовнішнього ключа команди
            existingUser.TeamId = user.TeamId;

            // Оновлення навігаційних властивостей (теги)
            existingUser.Tags = user.Tags;

            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();
        }


        public async Task<bool> Delete(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;
            
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
        //public async Task<UserEntity?> GetById(Guid id)
        //{
        //    return await _context.Users
        //        .Include(u => u.AdministeredTeams)
        //        .Include(u => u.CreatedTasks)
        //        .Include(u => u.AssignedTasks)
        //        .Include(u => u.Projects)
        //        .Include(u => u.Categories)
        //        .Include(u => u.Tags)
        //        .Include(u => u.Notifications)
        //        .Include(u => u.Team)
        //        .FirstOrDefaultAsync(u => u.Id == id);
        //}

        public async Task<User?> GetByEmailForLogin(string email)
        {
            var userEntity = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);

            if (userEntity == null)
            {
                return null;
            }

            return new User(userEntity.Id, userEntity.FullName, userEntity.Email,
                userEntity.PasswordHash, userEntity.CreatedAt);
        }

        
        //public async Task<Guid> Update(Guid id, string fullName, string email, string passwordHash, DateTime createdAt)
        //{
        //    await _context.Users
        //        .Where(u => u.Id == id)
        //        .ExecuteUpdateAsync(s => s
        //        .SetProperty(u => u.FullName, fullName)
        //        .SetProperty(u => u.Email, email)
        //        .SetProperty(u => u.PasswordHash, passwordHash)
        //        .SetProperty(u => u.CreatedAt, createdAt));

        //    return id;
        //}
      
        //public async Task<Guid> Delete(Guid id)
        //{
        //    int affectedRows = await _context.Users
        //        .Where(u => u.Id == id)
        //        .ExecuteDeleteAsync();

        //    if (affectedRows == 0)
        //        throw new KeyNotFoundException($"User with ID {id} not found.");

        //    return id;
        //}
    }
}
