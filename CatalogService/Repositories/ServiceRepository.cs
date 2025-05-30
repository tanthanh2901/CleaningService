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
            var service = await dbContext.Services
                        .Include(s => s.Options)
                        .FirstOrDefaultAsync(s => s.ServiceId == serviceId);

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

        public async Task<IEnumerable<ServiceDto>> GetServicesByCategory(int categoryId)
        {
            var services = await dbContext.Services
                .Include(x => x.Category)
                .Include(s => s.Options)
                .Where(x => (x.CategoryId == categoryId))
                .ToListAsync();

            return mapper.Map<IEnumerable<ServiceDto>>(services);
        }

        public async Task UpdateService(int serviceId, ServiceDtoForUpdate dto)
        {
            var service = await dbContext.Services
                .Include(s => s.Options)
                .FirstOrDefaultAsync(s => s.ServiceId == serviceId);

            mapper.Map(dto, service);

            dbContext.ServiceOptions.RemoveRange(service.Options);

            service.Options = dto.Options.Select(opt => mapper.Map<ServiceOption>(opt)).ToList();
            
            await dbContext.SaveChangesAsync();
        }

        public async Task<PaginationDto<ServiceDto>> GetPaginatedServices(int pageNumber, int pageSize)
        {
            var query = dbContext.Services
                .Include(s => s.Category)
                .Include(s => s.Options)
                .AsQueryable();

            var result = await query.ToPaginatedListAsync(pageNumber, pageSize);

            return mapper.Map<PaginationDto<ServiceDto>>(result);
        }

        public async Task<PaginationDto<ServiceDto>> SearchProduct(string searchQuery, int pageNumber, int pageSize)
        {
            IQueryable<Service> query = dbContext.Set<Service>();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(p =>
                    p.Name.Contains(searchQuery) ||
                    p.Description.Contains(searchQuery));
            }

            var result = await query.ToPaginatedListAsync(pageNumber, pageSize);

            return mapper.Map<PaginationDto<ServiceDto>>(result);
        }

    }
}
