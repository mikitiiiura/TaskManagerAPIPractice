using MediatR;
using TaskManagerAPIPractice.Contracts.Response;
using TaskManagerAPIPractice.Contracts;
using TaskManagerAPIPractice.DataAccess.abstruction;
using TaskManagerAPIPractice.Core.Model;

namespace TaskManagerAPIPractice.Application.Handlers.ProjectHandler
{
    public class GetAllProjectsHandler : IRequestHandler<GetUserProjectQuery, List<ProjectResponse>>
    {
        private readonly IProjectsRepository _projectsRepository;

        public GetAllProjectsHandler(IProjectsRepository projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }

        public async Task<List<ProjectResponse>> Handle(GetUserProjectQuery request, CancellationToken cancellationToken)
        {
            var project = await _projectsRepository.GetAllByUser(request.UserId);

            return project.Select(p => new ProjectResponse(p)).ToList();
        }
    }
}
