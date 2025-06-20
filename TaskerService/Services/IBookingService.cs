using TaskerService.Dtos;

namespace TaskerService.Services
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingDto>> GetBookingsByTaskerId(int taskerId);
        Task<BookingDetailsDto> GetBookingByIdAsync(int orderId);
    }
}
