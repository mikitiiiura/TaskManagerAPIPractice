using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagerAPIPractice.Contracts.Response;

namespace TaskManagerAPIPractice.Application.Contracts.Command
{
    public record CreateTaskCommand(
        string Title,
        string Description,
        int Status,
        int Priority,
        DateTime? DeadLine,
        Guid? TaskAssignedToId,
        Guid? CategoryId,
        Guid? ProjectId,
        List<Guid> Tags,
        Guid? TeamId,
        Guid TaskCreatedById
    ) : IRequest<TaskResponse>;
}
