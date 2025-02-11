using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.DataAccess.Configuration
{
    public class TaskConfiguration : IEntityTypeConfiguration<TaskEntity>
    {
        public void Configure(EntityTypeBuilder<TaskEntity> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Title).IsRequired().HasMaxLength(255);

            builder.HasOne(t => t.TaskCreatedBy)
                .WithMany(u => u.CreatedTasks)
                .HasForeignKey(t => t.TaskCreatedById)
                .OnDelete(DeleteBehavior.Restrict); // Завдання не може бути без автора

            builder.HasOne(t => t.TaskAssignedTo)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.TaskAssignedToId)
                .OnDelete(DeleteBehavior.SetNull); // Якщо користувач видаляється, завдання залишаються без виконавця

            builder.HasOne(t => t.Category)
                .WithMany(c => c.Tasks)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Завдання не може бути без категорії

            builder.HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.SetNull); // Якщо проєкт видаляється, завдання залишаються без проєкту

            builder.HasMany(t => t.Tags)
                .WithMany(tag => tag.Tasks);

            builder.HasMany(t => t.Notifications)
                .WithOne(n => n.Task)
                .HasForeignKey(n => n.TaskId);
        }
    }
}
