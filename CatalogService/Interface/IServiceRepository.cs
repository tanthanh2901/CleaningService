using CatalogService.Entities;

namespace CatalogService.Interface
{
    public interface IServiceRepository
    {
        Task<Service> AddService(Service service);
        Task<List<Service>> GetServices();
        Task<Service> GetServiceById(int serviceId);
        Task<IEnumerable<Service>> GetServicesByCategory(int categoryId);
        Task UpdateService(Service service);
        Task DeleteService(int serviceId);
    }

}
