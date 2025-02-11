namespace TaskManagerAPIPractice.Core.Model
{
    public class Project
    {
        public Project(Guid id, string title, string description, DateTime startDate, DateTime? endDate, ProjectStatus status)
        {
            Id = id;
            Title = title;
            Description = description;
            StartDate = startDate == default ? DateTime.UtcNow : startDate; // Переконуємося, що не пусте
            EndDate = endDate;
            Status = status;
        }
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        ///<summary> Статус проекту(активний, на паузі, завершений, архівний) </summary> 
        public ProjectStatus Status { get; set; } = ProjectStatus.Active; 

        public static (Project Project, string Error) Create(Guid id, string title, string description, DateTime startDate, DateTime? endDate, ProjectStatus status)
        {
            var error = string.Empty;

            if (string.IsNullOrWhiteSpace(title))
                error += "Title cannot be empty.\n";

            if (string.IsNullOrWhiteSpace(description))
                error += "Description cannot be empty.\n";

            if (endDate.HasValue && endDate < startDate)
                error += "End date cannot be earlier than start date.\n";

            var project = new Project(id, title, description, startDate, endDate, status);
            return (project, error);
        }
    }
    public enum ProjectStatus
    {
        Active,
        OnHold,
        Completed,
        Archived
    }

}
