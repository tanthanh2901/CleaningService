using CartService.Extensions;
using CartService.Interface;
using CartService.Models;

namespace CartService.Services
{
    public class ServiceCatalogService : IServiceCatalogService
    {
        private readonly HttpClient client;

        public ServiceCatalogService(HttpClient client)
        {
            this.client = client;
        }

        public async Task<Service> GetService(int serviceId)
        {
            var response = await client.GetAsync($"/api/services/{serviceId}");
            return await response.ReadContentAs<Service>();
        }
    }
}
