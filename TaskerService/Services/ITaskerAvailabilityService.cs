using TaskerService.Dtos;

namespace TaskerService.Services
{
    public interface ITaskerAvailabilityService
    {
        Task<bool> IsTaskerAvailableAsync(int taskerId, DateTime startTime, DateTime endTime);
        Task<List<TaskerWithCategoriesDto>> GetAvailableTaskersByCategoryAsync(int categoryId, DateTime startTime, DateTime endTime);
        Task CreateUnavailabilitySlotAsync(int taskerId, DateTime startTime, DateTime endTime, int bookingId, string reason);
        Task RemoveUnavailabilitySlotAsync(int bookingId);
        Task<List<DateTime>> GetTaskerAvailableTimeSlotsAsync(int taskerId, DateTime date);
    }
}
