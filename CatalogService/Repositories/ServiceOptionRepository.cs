using CatalogService.DbContexts;
using CatalogService.Entities;
using CatalogService.Interface;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Repositories
{
    public class ServiceOptionRepository : IServiceOptionRepository
    {
        private readonly CatalogDbContext dbContext;

        public ServiceOptionRepository(CatalogDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<ServiceOption>> GetByServiceOptionsAsync()
        {
            return await dbContext.ServiceOptions
                .ToListAsync();
        }

        public async Task CreateAsync(ServiceOption serviceOption)
        {
            dbContext.ServiceOptions.Add(serviceOption);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(ServiceOption serviceOption)
        {
            dbContext.ServiceOptions.Update(serviceOption);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(List<int> ids)
        {
            var optionsToDelete = await dbContext.ServiceOptions
                .Where(o => ids.Contains(o.ServiceOptionId))
            .ToListAsync();

            dbContext.ServiceOptions.RemoveRange(optionsToDelete);
            await dbContext.SaveChangesAsync();
        }
    }
}
