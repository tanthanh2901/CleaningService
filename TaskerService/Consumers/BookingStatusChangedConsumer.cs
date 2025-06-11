using MassTransit;
using MessageBus.IntegrationEvents;
using Microsoft.EntityFrameworkCore;
using TaskerService.DbContexts;
using TaskerService.Entities;

namespace TaskerService.Consumers
{
    public class BookingStatusChangedConsumer : IConsumer<BookingStatusChangedEvent>
    {
        private readonly TaskerDbContext _dbContext;
        private readonly ILogger<BookingStatusChangedConsumer> _logger;

        public BookingStatusChangedConsumer(
            ILogger<BookingStatusChangedConsumer> logger, TaskerDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<BookingStatusChangedEvent> context)
        {
            var message = context.Message;
            try
            {
                _logger.LogInformation("Processing BookingStatusChangedEvent for booking {BookingId}", message.BookingId);

                var booking = await _dbContext.Bookings
                    .FirstOrDefaultAsync(b => b.BookingId == message.BookingId);
                
                if (booking == null)
                {
                    _logger.LogWarning("Booking {BookingId} not found", message.BookingId);
                    return;
                }

                var bookingStatus = message.NewStatus.ToLower();


                switch (bookingStatus)
                {
                    case "canceled":
                        booking.BookingStatus = BookingStatus.Canceled;
                        break;
                    default:
                        _logger.LogWarning("Unknown booking status: {Status}", message.NewStatus);
                        return;
                }

                _dbContext.Bookings.Update(booking);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing BookingStatusChangedEvent for booking {BookingId}", 
                    message.BookingId);
                throw;
            }
        }
    }
}
