﻿using CatalogService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.DbContexts
{
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
        {

        }

        public DbSet<Service> Services { get; set; } = default!;
        public DbSet<Category> Categories { get; set; } = default!;
        public DbSet<ServiceOption> ServiceOptions { get; set; } = default!;
        public DbSet<DurationConfig> DurationConfigs { get; set; } = default!;
        public DbSet<PremiumService> PremiumServices { get; set; } = default!;
        public DbSet<AddonService> AddonServices { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
