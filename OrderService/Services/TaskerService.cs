using OrderService.Dtos;
using OrderService.Extensions;

namespace OrderService.Services
{
    public class TaskerService : ITaskerService
    {
        private readonly HttpClient client;

        public TaskerService(HttpClient client)
        {
            this.client = client;
        }

        public async Task<TaskerDto> GetTaskerById(int taskerId)
        {
            var response = await client.GetAsync($"/api/taskers/by-taskerId/{taskerId}");
            return await response.ReadContentAs<TaskerDto>();
        }
    }
}
