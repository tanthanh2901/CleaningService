using TaskerService.Dtos;
using TaskerService.Entities;
using TaskerService.Models;

namespace TaskerService.Services
{
    public interface ITaskerService
    {
        Task CreateAsync(Tasker tasker);
        Task<Tasker> GetTaskerByTaskerId(int taskerId);
        Task<Tasker> GetTaskerByUserId(int UserId);
        Task<List<TaskerWithCategoriesDto>> GetAvailableTaskersByCategory(int categoryId);
        Task<bool> SetAvailable(int userId);
        //Task<bool> ConfirmBooking(int bookingId);
        //Task<bool> CompletedBooking(int bookingId);
        //Task<bool> StartBooking(int bookingId);
        Task<bool> UpdateBookingStatus(UpdateBookingStatusModel model);


    }
}
