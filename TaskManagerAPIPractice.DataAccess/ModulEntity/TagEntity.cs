
using System.Text.Json.Serialization;

namespace TaskManagerAPIPractice.DataAccess.ModulEntity
{
    public class TagEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid? TagCreatedById { get; set; } // Користувач, який створив тег
        public UserEntity? TagCreatedBy { get; set; } // Користувач, який створив тег
        public List<TaskEntity> Tasks { get; set; } = []; // Список завдань, до яких прив’язаний тег

    }
}
