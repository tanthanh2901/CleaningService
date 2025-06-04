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
        private readonly IMapper mapper;


        public ServiceRepository(CatalogDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<Service> AddService(Service service)
        {
            await dbContext.Services.AddAsync(service);
            await dbContext.SaveChangesAsync();
            return service;
        }

        public async Task DeleteService(int serviceId)
        {
            var service = await GetByIdWithOptionsAsync(serviceId);

            dbContext.ServiceOptions.RemoveRange(service.Options);
            dbContext.Services.Remove(service);

            await dbContext.SaveChangesAsync();
        }

        public async Task<ServiceDto> GetServiceById(int serviceId)
        {           
            var service = await dbContext.Services
                .Include(s => s.Category)
                .Include(s => s.Options)
                .FirstOrDefaultAsync(s => s.ServiceId == serviceId);

            return mapper.Map<ServiceDto>(service);
        }

        public async Task<List<ServiceDto>> GetServices()
        {
            var listService = await dbContext.Services.ToListAsync();
            return mapper.Map<List<ServiceDto>>(listService);
        }


        //
        public async Task<Service> GetByIdAsync(int serviceId)
        {
            return await dbContext.Services.FindAsync(serviceId);
        }

        public async Task<Service> GetByIdWithOptionsAsync(int serviceId)
        {
            return await dbContext.Services
                           .Include(s => s.Options)
                           .FirstOrDefaultAsync(s => s.ServiceId == serviceId);
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
    }
}
