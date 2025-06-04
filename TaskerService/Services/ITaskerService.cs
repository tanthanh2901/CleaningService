using TaskerService.Dtos;
using TaskerService.Entities;

namespace TaskerService.Services
{
    public interface ITaskerService
    {
        Task CreateAsync(Tasker tasker);
        Task<List<Booking>> GetTaskerBookings(int taskerId);
        Task<Tasker> GetTaskerByTaskerId(int taskerId);
        Task<Tasker> GetTaskerByUserId(int UserId);
        Task<List<TaskerWithCategoriesDto>> GetAvailableTaskersByCategory(int categoryId);
        Task<bool> SetAvailable(int userId);
        Task<bool> ConfirmBooking(int bookingId);
        Task<bool> CompletedBooking(int bookingId);
        Task<bool> StartBooking(int bookingId);
    }
}
