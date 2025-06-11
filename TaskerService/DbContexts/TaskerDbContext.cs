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
        public DbSet<Booking> Bookings { get; set; } = default!;
        public DbSet<TaskerCategory> TaskerCategories { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TaskerCategory>()
                .HasKey(tc => new { tc.TaskerId, tc.CategoryId });

            modelBuilder.Entity<TaskerCategory>()
                .HasOne(tc => tc.Tasker)
                .WithMany(t => t.TaskerCategories)
                .HasForeignKey(tc => tc.TaskerId);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Tasker)
                .WithMany()
                .HasForeignKey(b => b.TaskerId);

            //modelBuilder.Entity<Booking>(entity =>
            //{
            //    entity.Property(b => b.BookingStatus)
            //        .HasMaxLength(50) // Optional: set max length
            //        .IsUnicode()       // Use NVARCHAR
            //        .IsRequired();     // Optional: make non-nullable
            //});
        }
    }
}
