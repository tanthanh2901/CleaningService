using Shared.Extensions;
using TaskerService.Dtos;
using TaskerService.Entities;

namespace TaskerService.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient client;

        public CatalogService(HttpClient client)
        {
            this.client = client;
        }

        public async Task<CategoryDto> GetCategory(int id)
        {
            var response = await client.GetAsync($"/api/categories/{id}");
            return await response.ReadContentAs<CategoryDto>();
        }
    }
}
