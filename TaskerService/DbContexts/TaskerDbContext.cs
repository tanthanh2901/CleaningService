using Microsoft.EntityFrameworkCore;
using TaskerService.Entities;

namespace TaskerService.DbContexts
{
    public class TaskerDbContext : DbContext
    {
        public TaskerDbContext(DbContextOptions<TaskerDbContext> options) : base(options)
        {
        }

        public DbSet<Tasker> Taskers { get; set; } = default!;
        public DbSet<TaskerCategory> TaskerCategories { get; set; } = default!;
        public DbSet<TaskerAvailabilitySlot> TaskerAvailabilitySlots { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TaskerCategory>()
                .HasKey(tc => new { tc.TaskerId, tc.CategoryId });

            modelBuilder.Entity<TaskerCategory>()
                .HasOne(tc => tc.Tasker)
                .WithMany(t => t.TaskerCategories)
                .HasForeignKey(tc => tc.TaskerId);

        }
    }
}
