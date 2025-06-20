using OrderService.Entities;

namespace OrderService.Services
{
    public interface ICatalogService
    {
        Task<Service> GetService(int id);
        Task<DurationConfig> GetDurationConfig(int durationsId);
        Task<IEnumerable<PremiumService>> GetPremiumServicesByIdsAsync(List<int> premiumServiceIds);
    }
}
