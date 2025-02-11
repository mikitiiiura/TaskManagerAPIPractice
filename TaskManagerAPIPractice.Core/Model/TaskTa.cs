namespace TaskManagerAPIPractice.Core.Model
{
    public class TaskTa
    {
        public TaskTa(Guid id, string title, string description, TaskStatus status, TaskPriority priority, DateTime? deadLine, DateTime createdAt)
        {
            Id = id;
            Title = title;
            Description = description;
            Status = status;
            Priority = priority;
            DeadLine = deadLine;
            CreatedAt = createdAt;
        }
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskStatus Status { get; set; } = TaskStatus.Pending;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public DateTime? DeadLine { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public static (TaskTa TaskTa, string Error) Create(Guid id, string title, string description, TaskStatus status, TaskPriority priority, DateTime? deadLine, DateTime createdAt)
        {
            var error = string.Empty;
            if (string.IsNullOrWhiteSpace(title))
                error += "Task title cannot be empty.\n";

            if (string.IsNullOrWhiteSpace(description))
                error += "Task description cannot be empty.\n";

            var task = new TaskTa(id, title, description, status, priority, deadLine, createdAt);
            return (task, error);
        }
    }
    public enum TaskStatus
    {
        Pending,  //(0)В очікуванні,
        InProgress, //(1)В роботі
        Completed, //(2)Завершено
        Archived  //(3)Архів або Задеплойено
    }
    public enum TaskPriority
    {
        Low,  //(0)
        Medium,//(1)
        High  //(2)
    }

}
