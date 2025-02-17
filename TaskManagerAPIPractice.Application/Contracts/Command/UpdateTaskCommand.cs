using MediatR;
using TaskManagerAPIPractice.Contracts.Response;

namespace TaskManagerAPIPractice.Application.Contracts.Command
{
    public record UpdateTaskCommand(
        Guid Id,
        string Title,
        string Description,
        int Status,
        int Priority,
        DateTime? DeadLine,
        Guid? TaskAssignedToId,
        Guid? CategoryId,
        Guid? ProjectId,
        List<Guid> Tags,
        Guid? TeamId
    ) : IRequest<TaskResponse>;
}
