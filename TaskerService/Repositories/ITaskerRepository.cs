using TaskerService.Entities;

namespace TaskerService.Repositories
{
    public interface ITaskerRepository
    {
        Task CreateTasker(Tasker tasker);
        Task<Tasker> GetTaskerByUserId(int userId);
        Task<Tasker> GetTaskerByTaskerId(int taskerId);
    }
}
