using MediatR;
using System.Text.Json.Serialization;
using TaskManagerAPIPractice.Contracts.Response;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.Contracts
{
    public record GetUserTasksQuery(Guid UserId) : IRequest<List<TaskResponse>>;
    public record GetUserTeamsQuery(Guid UserId) : IRequest<List<TeamResponse>>;
    public record GetUserProjectQuery(Guid UserId) : IRequest<List<ProjectResponse>>;
    public record TaskDetails(Guid Id, string Title);
    public record UserDetails(Guid Id, string Name);
    public record CategoryDetails(Guid Id, string Name);
    public record ProjectDetails(Guid Id, string Name);
    public record TagDetails(Guid Id, string Name);
    public record NotificationDetails(Guid Id, string Message);
    public record TeamDetails(Guid Id, string Name);
}