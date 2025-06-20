using EventBus;
using MassTransit;
using MessageBus.IntegrationEvents;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using TaskerService.DbContexts;
using TaskerService.Entities;

namespace TaskerService.Consumers
{
    public class BookingCreatedConsumer : IConsumer<BookingCreatedEvent>
    {
        private readonly TaskerDbContext _dbContext;
        private readonly IEventBus _eventBus;
        private readonly ILogger<BookingCreatedConsumer> _logger;


        public BookingCreatedConsumer(
            TaskerDbContext dbContext,
            ILogger<BookingCreatedConsumer> logger,
            IEventBus eventBus)
        {
            _dbContext = dbContext;
            _logger = logger;
            _eventBus = eventBus;
        }

        public async Task Consume(ConsumeContext<BookingCreatedEvent> context)
        {
            var message = context.Message;
            try
            {
                _logger.LogInformation("Processing BookingCreatedEvent for order {BookingId}", message.BookingId);

                var tasker = await _dbContext.Taskers
                    .FirstOrDefaultAsync(t => t.TaskerId == message.TaskerId);

                if (tasker == null)
                {
                    _logger.LogError("Tasker {TaskerId} not found", message.TaskerId);
                    return;
                }

                //tasker.IsAvailable = false;

                //_dbContext.Taskers.Update(tasker);
                //await _dbContext.SaveChangesAsync();

                _eventBus.PublishAsync(new BookingStatusChangedEvent()
                {
                    BookingId = message.BookingId,
                    NewStatus = Shared.Enums.BookingStatus.Assigned.ToString(),
                    ChangedAt = DateTime.Now
                });

                _logger.LogInformation("Successfully created booking for order {BookingId}", message.BookingId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing BookingCreatedEvent for order {BookingId}", message.BookingId);
                throw;
            }
        }
    }
} 