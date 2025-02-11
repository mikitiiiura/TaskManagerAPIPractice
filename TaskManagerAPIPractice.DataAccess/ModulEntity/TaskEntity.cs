using TaskManagerAPIPractice.Core.Model;

namespace TaskManagerAPIPractice.DataAccess.ModulEntity
{
    public class TaskEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Core.Model.TaskStatus Status { get; set; } = Core.Model.TaskStatus.Pending;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public DateTime? DeadLine { get; set; }
        public DateTime CreatedAt { get; set; }// DateTime.UtcNow;

        //Status, Priority, DeadLine, CreatedAt, TaskCreatedById, 

        // Зовнішні ключі
        public Guid TaskCreatedById { get; set; }
        public UserEntity TaskCreatedBy { get; set; } = null!; // Хто створив

        public Guid? TaskAssignedToId { get; set; }
        public UserEntity? TaskAssignedTo { get; set; } // Кому призначено

        public Guid? CategoryId { get; set; } // Категорія завдання
        public CategoryEntity? Category { get; set; } // Категорія завдання

        public Guid? ProjectId { get; set; } // Проект, до якого належить завдання
        public ProjectEntity? Project { get; set; } // Проект, до якого належить завдання

        public List<TagEntity> Tags { get; set; } = [];// Теги завдання

        public List<NotificationEntity> Notifications { get; set; } = []; // Список повідомлень

        // Навігаційна властивість (1 до 0..1)
        public Guid? TeamId { get; set; } // Команда, до якої належить завдання
        public TeamEntity? Team { get; set; }
    }
}
