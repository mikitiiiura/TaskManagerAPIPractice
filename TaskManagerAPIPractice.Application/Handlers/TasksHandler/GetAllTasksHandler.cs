
using MediatR;
using TaskManagerAPIPractice.Contracts;
using TaskManagerAPIPractice.Contracts.Response;
using TaskManagerAPIPractice.Core.Model;
using TaskManagerAPIPractice.DataAccess.abstruct;

namespace TaskManagerAPIPractice.Application.Handlers.TasksHandler
{
    public class GetAllTasksHandler : IRequestHandler<GetUserTasksQuery, List<TaskResponse>>
    {
        private readonly ITasksRepository _tasksRepository;

        public GetAllTasksHandler(ITasksRepository tasksRepository)
        {
            _tasksRepository = tasksRepository;
        }

        public async Task<List<TaskResponse>> Handle(GetUserTasksQuery request, CancellationToken cancellationToken)
        {
            var tasks = await _tasksRepository.GetAllByUser(request.UserId);
            return tasks.Select(task => new TaskResponse(task)).ToList();

            //return await _tasksRepository.GetAllByUser(userId);
        }
    }
}
