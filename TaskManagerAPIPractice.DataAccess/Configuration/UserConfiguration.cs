using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.DataAccess.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.FullName).IsRequired().HasMaxLength(255);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
            builder.HasIndex(u => u.Email).IsUnique();

            builder.HasMany(u => u.CreatedTasks)
                .WithOne(t => t.TaskCreatedBy)
                .HasForeignKey(t => t.TaskCreatedById)
                .OnDelete(DeleteBehavior.Restrict); // Забороняємо видаляти користувача, поки є створені ним завдання

            builder.HasMany(u => u.AssignedTasks)
                .WithOne(t => t.TaskAssignedTo)
                .HasForeignKey(t => t.TaskAssignedToId)
                .OnDelete(DeleteBehavior.SetNull); // Якщо користувач видаляється, завдання залишаються без виконавця

            builder.HasMany(u => u.Tags)
                .WithOne(t => t.TagCreatedBy)
                .HasForeignKey(t => t.TagCreatedById)
                .OnDelete(DeleteBehavior.SetNull); // Якщо користувач видаляється, теги залишаються без автора

            builder.HasMany(u => u.Categories)
                .WithOne(c => c.CategoryCreatedBy)
                .HasForeignKey(c => c.CategoryCreatedById)
                .OnDelete(DeleteBehavior.SetNull); // Якщо користувач видаляється, категорії залишаються без автора

            builder.HasMany(u => u.Notifications)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Видалення користувача не видаляє сповіщення

            builder.HasOne(u => u.Team)
                .WithMany(t => t.Users)
                .HasForeignKey(u => u.TeamId);
        }
    }
}
