using MediatR;
using TaskManagerAPIPractice.Contracts.Response;

namespace TaskManagerAPIPractice.Contracts.Request
{
    public record TaskAddUpdateRequest(
    string Title,
    string Description,
    int Status,
    int Priority,
    DateTime? DeadLine,
    //Guid TaskCreatedById, //
    Guid? TaskAssignedToId,
    Guid? CategoryId,
    Guid? ProjectId,
    List<Guid> Tags,
    Guid? TeamId
) : IRequest<TaskResponse>;
}
