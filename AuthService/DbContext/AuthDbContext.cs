using AuthService.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthService.DbContext
{
    public class AuthDbContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        public DbSet<AppUser> AppUsers { get; set; }

    }
}
