using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.Contracts.Response
{
    public record TaskResponse
    (
        Guid Id,
        string Title,
        string Description,
        int Status,
        int Priority,
        DateTime CreatedAt,
        DateTime? DeadLine,
        UserDetails TaskCreatedBy,
        UserDetails? TaskAssignedTo,
        CategoryDetails? Category,
        ProjectDetails? Project,
        List<TagDetails>? Tags,
        List<NotificationDetails>? Notifications,
        TeamDetails? Team
    )
    {
        public TaskResponse(TaskEntity task) : this(
            task.Id,
            task.Title,
            task.Description,
            (int)task.Status,
            (int)task.Priority,
            task.CreatedAt,
            task.DeadLine,
            task.TaskCreatedBy != null ? new UserDetails(task.TaskCreatedBy.Id, task.TaskCreatedBy.FullName) : null!,
            task.TaskAssignedTo != null ? new UserDetails(task.TaskAssignedTo.Id, task.TaskAssignedTo.FullName) : null,
            task.Category != null ? new CategoryDetails(task.Category.Id, task.Category.Title) : null,
            task.Project != null ? new ProjectDetails(task.Project.Id, task.Project.Title) : null,
            //task.Tags != null ? task.Tags.Select(tag => new TagDetails(tag.Id, tag.Name)).ToList() : new List<TagDetails>(),
            task.Tags != null ? task.Tags.Select(tag => new TagDetails(tag.Id, tag.Name)).ToList() : new List<TagDetails>(),
            task.Notifications != null ? task.Notifications.Select(notification => new NotificationDetails(notification.Id, notification.Message)).ToList() : new List<NotificationDetails>(),
            task.Team != null ? new TeamDetails(task.Team.Id, task.Team.Name) : null

        )
        { }

    }
}
