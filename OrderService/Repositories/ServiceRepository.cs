//using Microsoft.EntityFrameworkCore;
//using OrderService.DbContexts;
//using OrderService.Entities;
//using OrderService.Interface;

//namespace OrderService.Repositories
//{
//    public class ServiceRepository : IServiceRepository
//    {
//        private readonly OrderDbContext dbContext;

//        public ServiceRepository(OrderDbContext dbContext)
//        {
//            this.dbContext = dbContext;
//        }

//        public async Task<Service> GetServiceById(int serviceId)
//        {
//            return await dbContext.Services.FindAsync(serviceId);
//        }

//        public async Task<List<Service>> GetServices()
//        {
//            var listService = await dbContext.Services.ToListAsync();
//            return listService;
//        }
//    }

//}
