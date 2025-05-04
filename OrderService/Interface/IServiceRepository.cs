
using OrderService.Entities;

namespace OrderService.Interface
{
    public interface IServiceRepository
    {
        Task<List<Service>> GetServices();
        Task<Service> GetServiceById(int serviceId);
    }
}
