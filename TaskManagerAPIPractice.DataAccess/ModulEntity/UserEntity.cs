namespace TaskManagerAPIPractice.DataAccess.ModulEntity
{
    public class UserEntity
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Адміністровані команди
        public List<TeamEntity> AdministeredTeams { get; set; } = []; // ✅ Повернув це поле

        // Завдання, які користувач створив
        public List<TaskEntity> CreatedTasks { get; set; } = [];

        // Завдання, які призначені користувачу
        public List<TaskEntity> AssignedTasks { get; set; } = [];

        public List<ProjectEntity> Projects { get; set; } = [];
        public List<CategoryEntity> Categories { get; set; } = [];
        public List<TagEntity> Tags { get; set; } = [];
        public List<NotificationEntity> Notifications { get; set; } = [];

        // Команда користувача
        public Guid? TeamId { get; set; }
        public TeamEntity? Team { get; set; }
    }

}
