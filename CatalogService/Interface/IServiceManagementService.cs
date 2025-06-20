using CatalogService.Entities;

namespace CatalogService.Interface
{
    public interface IServiceManagementService
    {
        Task<Service?> GetServiceAsync(int serviceId);
        Task<IEnumerable<DurationConfig>> GetDurationConfigsAsync(int serviceId);
        Task<DurationConfig?> GetDurationConfigAsync(int durationConfigId);
        Task<IEnumerable<PremiumService>> GetPremiumServicesAsync(int serviceId);
        Task<IEnumerable<PremiumService>> GetPremiumServicesByIdsAsync(List<int> premiumServiceIds);
        Task<IEnumerable<AddonService>> GetAddonServicesAsync();
        Task<IEnumerable<AddonService>> GetAddonServicesByIdsAsync(List<int> addonServiceIds);
        Task<IEnumerable<ServiceOption>> GetServiceOptionsAsync();
        Task<IEnumerable<ServiceOption>> GetServiceOptionsByIdsAsync(List<int> serviceOptionIds);
    }
}
