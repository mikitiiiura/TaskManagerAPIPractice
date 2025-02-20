
using MediatR;
using TaskManagerAPIPractice.Contracts.Response;
using TaskManagerAPIPractice.Contracts;
using TaskManagerAPIPractice.DataAccess.abstruction;
using System.Linq;

namespace TaskManagerAPIPractice.Application.Handlers.Teamhandler
{
    public class GetAllTeamHendler : IRequestHandler<GetUserTeamsQuery, List<TeamResponse>>
    {
        private readonly ITeamRepository _teamRepository;

        public GetAllTeamHendler(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        public async Task<List<TeamResponse>> Handle(GetUserTeamsQuery request, CancellationToken cancellationToken)
        {
            var team = await _teamRepository.GetAllByUser(request.UserId);
            return team.Select(team => new TeamResponse(team)).ToList();
        }
    }
}
