using CatalogService.DbContexts;
using CatalogService.Dtos;
using CatalogService.Extensions;
using CatalogService.Interface;
using CatalogService.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper;
using CatalogService.AWSS3;
using Shared.Entities;
using Shared.Extensions;
using Microsoft.EntityFrameworkCore.Storage;

namespace CatalogService.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly CatalogDbContext dbContext;

        public ServiceRepository(CatalogDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Service> AddService(Service service)
        {
            await dbContext.Services.AddAsync(service);
            await dbContext.SaveChangesAsync();
            return service;
        }

        public async Task UpdateAsync(Service service)
        {
            dbContext.Services.Update(service);
            await dbContext.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await dbContext.Database.BeginTransactionAsync();
        }

        public async Task<Service> GetServiceByIdAsync(int serviceId)
        {
            return await dbContext.Services.FindAsync(serviceId);
        }

        public async Task<List<Service>> GetServices()
        {
            return await dbContext.Services.ToListAsync();
        }
    }
}
