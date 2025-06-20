using System.Net.Http.Headers;
using System.Net.Http;
using Shared.Extensions;
using TaskerService.Dtos;

namespace TaskerService.Services
{
    public class BookingService : IBookingService
    {
        private readonly HttpClient client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BookingService(HttpClient client, IHttpContextAccessor httpContextAccessor)
        {
            this.client = client;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<BookingDetailsDto> GetBookingByIdAsync(int orderId)
        {
           

            var response = await client.GetAsync($"/api/orders/{orderId}");
            return await response.ReadContentAs<BookingDetailsDto>();
        }

        public async Task<IEnumerable<BookingDto>> GetBookingsByTaskerId(int taskerId)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
            }

            var response = await client.GetAsync($"/api/orders/tasker/{taskerId}");
            return await response.ReadContentAs<IEnumerable<BookingDto>>();
        }
    }
}
