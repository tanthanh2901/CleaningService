using CatalogService.Dtos;

namespace CatalogService.Interface
{
    public interface IServiceService
    {
        Task<IEnumerable<ServiceDto>> GetServicesAsync();
        Task<ServiceDto> GetServiceById(int serviceId);
        Task<ServiceDtoForCreate> AddService(ServiceDtoForCreate service);
        Task<bool> UpdateAsync(int serviceId, ServiceDtoForUpdate dto);
        Task DeleteService(int serviceId);

        Task<IEnumerable<ServiceDto>> GetServicesByCategory(int categoryId);
        Task<PaginationDto<ServiceDto>> SearchProduct(string searchQuery, int pageNumber, int pageSize);
        Task<PaginationDto<ServiceDto>> GetPaginatedServices(int pageNumber, int pageSize);

    }
}
