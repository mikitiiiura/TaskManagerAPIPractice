using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.DataAccess.abstruction
{
    public interface ITeamRepository
    {
        Task<List<TeamEntity>> GetAll();
        Task<List<TeamEntity>> GetAllByUser(Guid userId);
    }
}