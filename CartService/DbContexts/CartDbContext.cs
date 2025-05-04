using Microsoft.EntityFrameworkCore;
using CartService.Models;

namespace CatalogService.DbContexts
{
    public class CartDbContext : DbContext
    {
        public CartDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
        {

        }
        public DbSet<Service> Services { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}
