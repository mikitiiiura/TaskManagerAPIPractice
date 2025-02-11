using TaskManagerAPIPractice.Core.Model;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.Contracts
{

    public record UserRequest(
        string? FullName,
        string? Email,
        Guid? TeamId,
        List<Guid>? Tags,
        List<Guid>? Tasks
    );

    public record UserResponse(
        Guid Id,
        string FullName,
        string Email,
        DateTime CreatedAt,
        TeamResp? Team,
        List<TagResp>? Tags,
        int TasksCount
    );

    public record TeamResp(Guid Id, string Name);
    public record TagResp(Guid Id, string Name);


    public record TagRequest(string Name, Guid? TagCreatedById);
    public record TagResponse(Guid Id, string Name, UserDetails? CreatedBy, int TaskCount);
    public record NotificationRequest(
        string Message,
        bool IsRead,
        Guid UserId,
        Guid? TaskId
    );

    public record NotificationResponse(
        Guid Id,
        string Message,
        bool IsRead,
        DateTime CreatedAt,
        UserDetails User,
        TaskDetails? Task
    );
    public record CategoryRequest
    (
        string Title,
        Guid CategoryCreatedById
    );

    public record CategoryResponse
    (
        Guid Id,
        string Title,
        UserDetails CategoryCreatedBy,
        int TaskCount
    )
    {
        public CategoryResponse(CategoryEntity category) : this(
            category.Id,
            category.Title,
            new UserDetails(category.CategoryCreatedBy.Id, category.CategoryCreatedBy.FullName),
            category.Tasks.Count)
        { }
    }
    public record TaskRequest
    (
        Guid Id,
        string Title,
        string Description,
        int Status,
        int Priority,
        DateTime DeadLine,
        DateTime CreatedAt,
        Guid TaskCreatedById,
        Guid? TaskAssignedToId,
        Guid? CategoryId,
        Guid? ProjectId,
        List<Guid>? Tags,
        List<Guid>? Notifications,
        Guid? TeamId
    )
    {
        public TaskRequest(string title, string description, int status, int priority, DateTime deadLine, Guid taskCreatedById,
            Guid? taskAssignedToId = null, Guid? categoryId = null, Guid? projectId = null,
            List<Guid>? tags = null, List<Guid>? notifications = null, Guid? teamId = null)
            : this(Guid.NewGuid(), title, description, status, priority, deadLine, DateTime.UtcNow, taskCreatedById,
                taskAssignedToId, categoryId, projectId, tags, notifications, teamId)
        { }
    }

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
            task.Tags != null ? task.Tags.Select(tag => new TagDetails(tag.Id, tag.Name)).ToList() : new List<TagDetails>(),
            task.Notifications != null ? task.Notifications.Select(notification => new NotificationDetails(notification.Id, notification.Message)).ToList() : new List<NotificationDetails>(),
            task.Team != null ? new TeamDetails(task.Team.Id, task.Team.Name) : null
        )
        { }

    }

    public record TaskDetails(Guid Id, string Title);
    public record UserDetails(Guid Id, string Name);
    public record CategoryDetails(Guid Id, string Name);
    public record ProjectDetails(Guid Id, string Name);
    public record TagDetails(Guid Id, string Name);
    public record NotificationDetails(Guid Id, string Message);
    public record TeamDetails(Guid Id, string Name);

    public record ProjectRequest(string Title, string Description, DateTime? EndDate, int Status, Guid? TeamId, Guid? ProjectCreatedById);

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
