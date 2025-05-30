using Microsoft.AspNetCore.Identity;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shared.Entities;

namespace Shared.DbContexts
{
    public class UserDbContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var hasher = new PasswordHasher<AppUser>();

            // Seed Roles
            builder.Entity<AppRole>().HasData(
                new AppRole { Id = 1, Name = "admin", NormalizedName = "ADMIN" },
                new AppRole { Id = 2, Name = "customer", NormalizedName = "CUSTOMER" },
                new AppRole { Id = 3, Name = "tasker", NormalizedName = "TASKER" }
            );

            // Seed Users
            var adminUser = new AppUser
            {
                Id = 1,
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                SecurityStamp = Guid.NewGuid().ToString("D"),
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Aa204415627.")
            };

            var normalUser = new AppUser
            {
                Id = 2,
                Email = "user@gmail.com",
                NormalizedEmail = "USER@GMAIL.COM",
                UserName = "user",
                NormalizedUserName = "USER",
                SecurityStamp = Guid.NewGuid().ToString("D"),
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Aa204415627.")
            };

            var tasker = new AppUser
            {
                Id = 3,
                Email = "tasker@gmail.com",
                NormalizedEmail = "TASKER@GMAIL.COM",
                UserName = "tasker",
                NormalizedUserName = "TASKER",
                SecurityStamp = Guid.NewGuid().ToString("D"),
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Aa204415627.")
            };

            builder.Entity<AppUser>().HasData(adminUser, normalUser, tasker);

            // Seed UserRoles (Assign admin to admin, customer to user)
            builder.Entity<IdentityUserRole<int>>().HasData(
                new IdentityUserRole<int> { UserId = 1, RoleId = 1 }, // admin -> admin
                new IdentityUserRole<int> { UserId = 2, RoleId = 2 },  // user -> customer
                new IdentityUserRole<int> { UserId = 3, RoleId = 3 }  // user -> customer
            );
        }
    }
}

