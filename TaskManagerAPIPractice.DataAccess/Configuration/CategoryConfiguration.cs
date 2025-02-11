

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.DataAccess.Configuration
{
    public class CategoryConfiguration : IEntityTypeConfiguration<CategoryEntity>
    {
        public void Configure(EntityTypeBuilder<CategoryEntity> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Title).IsRequired().HasMaxLength(255);

            builder.
                 HasOne(c => c.CategoryCreatedBy)
                .WithMany(u => u.Categories)
                .HasForeignKey(c => c.CategoryCreatedById)
                .OnDelete(DeleteBehavior.Restrict); // Користувача не можна видалити, поки є категорії

            builder.
                   HasMany(c => c.Tasks)
                   .WithOne(t => t.Category)
                   .HasForeignKey(t => t.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict); // Категорію не можна видалити, поки є завдання
        }
    }
}
