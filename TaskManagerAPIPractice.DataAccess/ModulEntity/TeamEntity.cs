namespace TaskManagerAPIPractice.DataAccess.ModulEntity
{
    public class TeamEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;


        public Guid? AdminId { get; set; } // Адміністратор команди
        public UserEntity? Admin { get; set; } // Адміністратор команди

        public List<UserEntity> Users { get; set; } = []; // Учасники команди

        public List<TaskEntity> Tasks { get; set; } = []; // Завдання, які належать команді

        public List<ProjectEntity> Projects { get; set; } = []; // Проекти, які належать команді

    }
}
