using CatalogService.Dtos;

namespace CatalogService.Interface
{
    public interface IServiceService
    {
        Task<ServiceDto> GetByIdWithtOptionsAsync(int id);
        Task<ServiceDto> GetServiceById(int serviceId);
        Task<ServiceDtoForCreate> AddService(ServiceDtoForCreate service);
        Task<ServiceDto> UpdateAsync(int id, ServiceDtoForUpdate serviceDto);
        Task DeleteService(int serviceId);

        Task<IEnumerable<ServiceDto>> GetServicesByCategory(int categoryId);
        Task<PaginationDto<ServiceDto>> SearchProduct(string searchQuery, int pageNumber, int pageSize);
        Task<PaginationDto<ServiceDto>> GetPaginatedServices(int pageNumber, int pageSize);

    }
}
