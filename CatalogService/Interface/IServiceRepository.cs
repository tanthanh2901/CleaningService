using CatalogService.Dtos;
using CatalogService.Entities;
using Shared.Extensions;

namespace CatalogService.Interface
{
    public interface IServiceRepository
    {
        Task<Service> AddService(Service service);
        Task<List<ServiceDto>> GetServices();
        Task<ServiceDto> GetServiceById(int serviceId);
        Task<IEnumerable<ServiceDto>> GetServicesByCategory(int categoryId);
        Task UpdateService(int serviceId, ServiceDtoForUpdate service);
        Task DeleteService(int serviceId);
        Task<PaginationDto<ServiceDto>> SearchProduct(string searchQuery, int pageNumber, int pageSize);
        Task<PaginationDto<ServiceDto>> GetPaginatedServices(int pageNumber, int pageSize);
    }
}
