using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.Contracts.Response
{
    public record TeamResponse
   (
       Guid Id,
       string Name,
       UserDetails Admin,
       List<UserDetails>? Users,
       int TaskCount,
       List<ProjectDetails>? Projects
   )
    {
        public TeamResponse(TeamEntity team) : this(
            team.Id,
            team.Name,
            team.Admin != null ? new UserDetails(team.Admin.Id, team.Admin.FullName) : null!,
            team.Users != null ? team.Users.Select(user => new UserDetails(user.Id, user.FullName)).ToList() : new List<UserDetails>(),
            team.Tasks.Count(),
            team.Projects != null ? team.Projects.Select(project => new ProjectDetails(project.Id, project.Title)).ToList() : new List<ProjectDetails>()
        )
        { }

    }
}
