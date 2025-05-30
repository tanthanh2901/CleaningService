using Microsoft.EntityFrameworkCore;
using TaskerService.DbContexts;
using TaskerService.Entities;

namespace TaskerService.Repositories
{
    public class TaskerRepository : ITaskerRepository
    {
        private readonly TaskerDbContext _context;

        public TaskerRepository(TaskerDbContext dbContext)
        {
            this._context = dbContext;
        }

        public async Task CreateTasker(Tasker tasker)
        {
            await _context.AddAsync(tasker);
            await _context.SaveChangesAsync();
        }

        public async Task<Tasker> GetTaskerById(int userId)
        {
            return await _context.Taskers.FirstOrDefaultAsync(t => t.UserId == userId);
        }
    }
}
