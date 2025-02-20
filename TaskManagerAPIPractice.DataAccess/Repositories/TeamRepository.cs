
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagerAPIPractice.DataAccess.abstruction;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.DataAccess.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly TaskAPIDbContext _context;
        private readonly ILogger<TeamRepository> _logger;

        public TeamRepository(TaskAPIDbContext context, ILogger<TeamRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Отримати всі команди
        public async Task<List<TeamEntity>> GetAll()
        {
            _logger.LogInformation("Fetching all team.");
            try
            {
                var team = await _context.Teams
                    .Include(t => t.AdminId)
                    .Include(t => t.Users)
                    .Include(t => t.Tasks)
                    .Include(t => t.Projects)
                    .ToListAsync();
                _logger.LogInformation("Successfully fetched all team.");
                return team;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all team.");
                throw;
            }
        }

        public async Task<List<TeamEntity>> GetAllByUser(Guid userId)
        {
            _logger.LogInformation("Fetching all team for user ID: {UserId}", userId);
            try
            {
                var team = await _context.Teams
                    .Where(t => t.AdminId == userId || t.Users.Any(u => u.Id == userId))
                    .Include(t => t.Admin)
                    .Include(t => t.Users)
                    .Include(t => t.Tasks)
                    .Include(t => t.Projects)
                    .AsNoTracking()
                    .ToListAsync();
                _logger.LogInformation("Successfully fetched all team for user ID: {UserId}", userId);
                return team;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all team for user ID: {UserId}", userId);
                throw;
            }
        }
    }
}
