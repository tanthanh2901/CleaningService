using CartService.Models;

namespace CartService.Interface
{
    public interface IServiceRepository
    {
        void AddService(Service theService);
        Task<bool> ServiceExists(int serviceId);
    }
}
