using TaskerService.Dtos;
using TaskerService.Entities;

namespace TaskerService.Services
{
    public interface ITaskerService
    {
        Task CreateAsync(Tasker tasker);
        Task<List<Booking>> GetTaskerBookings(int taskerId, string? status = null);

        Task<List<TaskerWithCategoriesDto>> GetAvailableTaskersByCategory(int categoryId);
        Task<bool> SetAvailable(int taskerId);
        Task<bool> ConfirmBooking(int bookingId);
        Task<bool> CompletedBooking(int bookingId);
    }
}
