using CatalogService.DbContexts;
using CatalogService.Entities;
using CatalogService.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Services
{
    public class ServiceManagementService : IServiceManagementService
    {
        private readonly CatalogDbContext _context;

        public ServiceManagementService(CatalogDbContext context)
        {
            _context = context;
        }

        public async Task<Service?> GetServiceAsync(int serviceId)
        {
            return await _context.Services
                .Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.ServiceId == serviceId);
        }

        public async Task<IEnumerable<DurationConfig>> GetDurationConfigsAsync(int serviceId)
        {
            return await _context.DurationConfigs
                .Where(d => d.ServiceId == serviceId)
                .ToListAsync();
        }

        public async Task<DurationConfig?> GetDurationConfigAsync(int durationConfigId)
        {
            return await _context.DurationConfigs
                .FirstOrDefaultAsync(d => d.DurationConfigId == durationConfigId);
        }

        public async Task<IEnumerable<PremiumService>> GetPremiumServicesAsync(int serviceId)
        {
            return await _context.PremiumServices
                .Where(p => p.ServiceId == serviceId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PremiumService>> GetPremiumServicesByIdsAsync([FromQuery] List<int> premiumServiceIds)
        {
            return await _context.PremiumServices
                .Where(p => premiumServiceIds.Contains(p.PremiumServiceId))
                .ToListAsync();
        }

        public async Task<IEnumerable<AddonService>> GetAddonServicesAsync()
        {
            return await _context.AddonServices
                .ToListAsync();
        }

        public async Task<IEnumerable<AddonService>> GetAddonServicesByIdsAsync(List<int> addonServiceIds)
        {
            return await _context.AddonServices
                .Where(a => addonServiceIds.Contains(a.AddonServiceId))
                .ToListAsync();
        }

        public async Task<IEnumerable<ServiceOption>> GetServiceOptionsAsync()
        {
            return await _context.ServiceOptions
                .ToListAsync();
        }

        public async Task<IEnumerable<ServiceOption>> GetServiceOptionsByIdsAsync(List<int> serviceOptionIds)
        {
            return await _context.ServiceOptions
                .Where(o => serviceOptionIds.Contains(o.ServiceOptionId))
                .ToListAsync();
        }
    }
}
