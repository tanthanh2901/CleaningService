using OrderService.Entities;

namespace OrderService.Services
{
    public interface ICatalogService
    {
        Task<Service> GetService(int id);
    }
}
