namespace TaskManagerAPIPractice.DataAccess.ModulEntity
{
    public class CategoryEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public Guid CategoryCreatedById { get; set; } // Користувач, який створив категорію
        public UserEntity? CategoryCreatedBy { get; set; } // Користувач, який створив категорію

        public List<TaskEntity> Tasks { get; set; } = []; // Список завдань у цій категорії

    }
}
