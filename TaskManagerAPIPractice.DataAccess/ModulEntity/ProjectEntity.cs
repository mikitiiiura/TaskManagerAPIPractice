using TaskManagerAPIPractice.Core.Model;

namespace TaskManagerAPIPractice.DataAccess.ModulEntity
{
    public class ProjectEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ProjectStatus Status { get; set; } = ProjectStatus.Active;

        // Зовнішні ключі та навігаційні властивості
        public Guid? TeamId { get; set; }
        public TeamEntity? Team { get; set; }

        public Guid? ProjectCreatedById { get; set; }
        public UserEntity? ProjectCreatedBy { get; set; }

        public List<TaskEntity> Tasks { get; set; } = [];
    }
}