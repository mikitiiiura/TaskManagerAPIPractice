using TaskManagerAPIPractice.Core.Model;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.Contracts.Response
{
    public record ProjectResponse(Guid Id, string Title, string Description, DateTime StartDate, DateTime? EndDate, ProjectStatus Status,
        TeamDetails? Team, UserDetails? ProjectCreatedBy, int TaskCount)
    {
        public ProjectResponse(ProjectEntity project) : this(
            project.Id,
            project.Title,
            project.Description,
            project.StartDate,
            project.EndDate,
            project.Status,
            project.Team != null ? new TeamDetails(project.Team.Id, project.Team.Name) : null,
            project.ProjectCreatedBy != null ? new UserDetails(project.ProjectCreatedBy.Id, project.ProjectCreatedBy.FullName) : null,
            project.Tasks.Count)
        { }
    }
}
