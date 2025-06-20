using CatalogService.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace CatalogService.Interface
{
    public interface IServiceRepository
    {
        Task<Service> GetServiceByIdAsync(int serviceId);
        Task UpdateAsync(Service service);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<Service> AddService(Service service);


        Task<List<Service>> GetServices();
    }
}
