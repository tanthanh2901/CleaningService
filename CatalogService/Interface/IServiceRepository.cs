using CatalogService.Dtos;
using CatalogService.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using Shared.Extensions;

namespace CatalogService.Interface
{
    public interface IServiceRepository
    {
        Task<Service> GetByIdAsync(int serviceId);
        Task<Service> GetByIdWithOptionsAsync(int serviceId);
        Task UpdateAsync(Service service);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<Service> AddService(Service service);


        Task<List<ServiceDto>> GetServices();
    }
}
