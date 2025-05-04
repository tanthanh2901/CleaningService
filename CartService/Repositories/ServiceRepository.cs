using CartService.Interface;
using CartService.Models;
using CatalogService.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CartService.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly CartDbContext dbContext;

        public ServiceRepository(CartDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void AddService(Service theService)
        {
            dbContext.Services.Add(theService);
        }

        public async Task<bool> ServiceExists(int serviceId)
        {
            return await dbContext.Services.AnyAsync(s => s.ServiceId == serviceId);
        }


    }
}
