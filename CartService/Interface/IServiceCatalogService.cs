using CartService.Models;

namespace CartService.Interface
{
    public interface IServiceCatalogService
    {
        Task<Service> GetService(int serviceId);
    }
}
