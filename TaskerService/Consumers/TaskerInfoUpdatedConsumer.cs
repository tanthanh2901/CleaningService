using MassTransit;
using MessageBus.IntegrationEvents;
using TaskerService.DbContexts;
using TaskerService.Services;

namespace TaskerService.Consumers
{
    public class TaskerInfoUpdatedConsumer : IConsumer<TaskerInfoUpdatedEvent>
    {
        private readonly ITaskerService _taskerService;
        private readonly TaskerDbContext _dbContext;

        public TaskerInfoUpdatedConsumer(ITaskerService taskerService, TaskerDbContext dbContext)
        {
            _taskerService = taskerService;
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<TaskerInfoUpdatedEvent> context)
        {
            var message = context.Message;
            var tasker = await _taskerService.GetTaskerByUserId(message.UserId);

            if (tasker == null) {
                return;
            }

            tasker.FullName = message.FullName;
            tasker.PhoneNumber = message.PhoneNumber;
            tasker.Avatar = message.Avatar;
            tasker.Address = message.Address;

            _dbContext.Taskers.Update(tasker);
            await _dbContext.SaveChangesAsync();
        }
    }
}
