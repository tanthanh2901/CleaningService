using System.Net.Http;
using MassTransit;
using OrderService.Entities;
using OrderService.Extensions;

namespace OrderService.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient client;

        public CatalogService(HttpClient client)
        {
            this.client = client;
        }

        public async Task<DurationConfig> GetDurationConfig(int durationsId)
        {
            var response = await client.GetAsync($"/api/services/durations/{durationsId}");
            return await response.ReadContentAs<DurationConfig>();
        }

        public async Task<IEnumerable<PremiumService>> GetPremiumServicesByIdsAsync(List<int> premiumServiceIds)
        {
            var ids = string.Join(",", premiumServiceIds);
            var response = await client.GetAsync($"/api/services/premiums?premiumServiceIds={ids}");
            return await response.ReadContentAs<List<PremiumService>>();
        }

        public async Task<Service> GetService(int id)
        {
            var response = await client.GetAsync($"/api/services/{id}");
            return await response.ReadContentAs<Service>();
        }   
    }
}
