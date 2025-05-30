using TaskerService.Dtos;
using TaskerService.Entities;

namespace TaskerService.Repositories
{
    public interface ITaskerRepository
    {
        Task CreateTasker(Tasker tasker);
        Task<Tasker> GetTaskerById(int userId);
    }
}
