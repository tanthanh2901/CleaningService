﻿using Microsoft.EntityFrameworkCore;
using PaymentService.Entities;



namespace PaymentService.DbContexts
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
        {
        }

        public DbSet<Payment> Payments { get; set; } = default!;

    }
}
