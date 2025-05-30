using Microsoft.EntityFrameworkCore;
using OrderService.Entities;

namespace OrderService.DbContexts
{
    public class BookingDbContext : DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options)
        {

        }

        public DbSet<Booking> Bookings { get; set; } = default!;
        public DbSet<BookingOption> BookingOptions { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
