using TaskManagerAPIPractice.Core.Model;

namespace TaskManagerAPIPractice.Persistence
{
    public interface IJwtProvider
    {
        string GenerateToken(User user);
    }
}