namespace TaskManagerAPIPractice.DataAccess.ModulEntity
{
    public class NotificationEntity
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; }

        public Guid UserId { get; set; } // Користувач, який отримав сповіщення
        public UserEntity? User { get; set; } // Користувач, який отримав сповіщення

        public Guid? TaskId { get; set; } // Завдання, пов’язане зі сповіщенням (може бути null)
        public TaskEntity? Task { get; set; } // Завдання, пов’язане зі сповіщенням
    }
}
