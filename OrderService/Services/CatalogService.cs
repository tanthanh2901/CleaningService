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

        public async Task<Service> GetService(int id)
        {
            var response = await client.GetAsync($"/api/services/{id}");
            return await response.ReadContentAs<Service>();
        }

      
    }
}
