
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.DataAccess.Configuration
{
    public class ProjectConfiguration : IEntityTypeConfiguration<ProjectEntity>
    {
        public void Configure(EntityTypeBuilder<ProjectEntity> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Title).IsRequired().HasMaxLength(255);

            builder.HasOne(p => p.Team)
                .WithMany(t => t.Projects)
                .HasForeignKey(p => p.TeamId)
                .OnDelete(DeleteBehavior.SetNull); // Якщо команду видаляють, проєкти залишаються без команди

            builder.HasOne(p => p.ProjectCreatedBy)
                .WithMany(u => u.Projects)
                .HasForeignKey(p => p.ProjectCreatedById)
                .OnDelete(DeleteBehavior.SetNull); // Якщо користувач видаляється, проєкт залишиться без автора
        }
    }
}
