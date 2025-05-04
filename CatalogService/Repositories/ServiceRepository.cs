using CatalogService.DbContexts;
using CatalogService.Interface;
using CatalogService.Entities;
using Microsoft.EntityFrameworkCore;

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

        public async Task DeleteService(int serviceId)
        {
            var service = await this.GetServiceById(serviceId);
            dbContext.Services.Remove(service);
            await dbContext.SaveChangesAsync();
        }

        public async Task<Service> GetServiceById(int serviceId)
        {
            return await dbContext.Services.FindAsync(serviceId);
        }

        public async Task<List<Service>> GetServices()
        {
            var listService = await dbContext.Services.ToListAsync();
            return listService;
        }

        public async Task<IEnumerable<Service>> GetServicesByCategory(int categoryId)
        {
            return await dbContext.Services
                .Include(x => x.Category)
                .Where(x => (x.CategoryId == categoryId)).ToListAsync();

        }

        public async Task UpdateService(Service service)
        {
            try
            {
                dbContext.Entry(service).State = EntityState.Modified;
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException("Concurrency issue occurred while updating", ex);
            }
        }
    }

}
