using Microsoft.EntityFrameworkCore;
using TaskManagerAPIPractice.DataAccess.Configuration;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.DataAccess
{
    public class TaskAPIDbContext : DbContext
    {
        public TaskAPIDbContext(DbContextOptions<TaskAPIDbContext> option) : base(option)
        {
        }

        public DbSet<CategoryEntity> CategoryEntities { get; set; }
        public DbSet<NotificationEntity> Notifications { get; set; }
        public DbSet<ProjectEntity> Projects { get; set; }
        public DbSet<TagEntity> Tags { get; set; }
        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<TeamEntity> Teams { get; set; }
        public DbSet<UserEntity> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectConfiguration());
            modelBuilder.ApplyConfiguration(new TagConfiguration());
            modelBuilder.ApplyConfiguration(new TaskConfiguration());
            modelBuilder.ApplyConfiguration(new TeamConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
