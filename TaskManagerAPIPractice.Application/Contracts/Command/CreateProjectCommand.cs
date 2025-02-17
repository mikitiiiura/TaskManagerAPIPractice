using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagerAPIPractice.Contracts.Response;

namespace TaskManagerAPIPractice.Application.Contracts.Command
{
    public record CreateProjectCommand
    (
        string Title, string Description, DateTime? EndDate, int Status, Guid? TeamId, Guid projectCreatedById
    ) : IRequest<ProjectResponse>;
}
