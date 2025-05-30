using MassTransit;
using MessageBus.IntegrationEvents;
using Microsoft.EntityFrameworkCore;
using TaskerService.DbContexts;
using TaskerService.Entities;

namespace TaskerService.Consumers
{
    public class BookingCreatedConsumer : IConsumer<BookingCreatedEvent>
    {
        private readonly TaskerDbContext _dbContext;
        private readonly ILogger<BookingCreatedConsumer> _logger;

        public BookingCreatedConsumer(
            TaskerDbContext dbContext,
            ILogger<BookingCreatedConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<BookingCreatedEvent> context)
        {
            var message = context.Message;
            try
            {
                _logger.LogInformation("Processing BookingCreatedEvent for order {BookingId}", message.BookingId);

                // Create booking record
                var booking = new Booking
                {
                    BookingId = message.BookingId,
                    TaskerId = message.TaskerId,
                    ScheduleTime = message.ScheduleTime,
                    Address = message.Address,
                    TotalAmount = message.TotalAmount,
                    BookingStatus = "Assigned",
                };

                var tasker = await _dbContext.Taskers
                    .FirstOrDefaultAsync(t => t.TaskerId == message.TaskerId);

                if (tasker == null)
                {
                    _logger.LogError("Tasker {TaskerId} not found", message.TaskerId);
                    return;
                }

                booking.Tasker = tasker;
                _dbContext.Bookings.Add(booking);
                await _dbContext.SaveChangesAsync();

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