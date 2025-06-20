using CatalogService.Entities;

namespace CatalogService.Interface
{
    public interface IServiceOptionRepository
    {
        Task<List<ServiceOption>> GetByServiceOptionsAsync();
        Task CreateAsync(ServiceOption serviceOption);
        Task UpdateAsync(ServiceOption serviceOption);
        Task DeleteRangeAsync(List<int> ids);
    }
}
