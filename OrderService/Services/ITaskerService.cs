using OrderService.Dtos;

namespace OrderService.Services
{
    public interface ITaskerService
    {
        Task<TaskerDto> GetTaskerById(int taskerId);
    }
}
