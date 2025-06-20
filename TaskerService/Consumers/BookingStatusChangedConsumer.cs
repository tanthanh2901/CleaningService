using MassTransit;
using MessageBus.IntegrationEvents;
using Microsoft.EntityFrameworkCore;
using TaskerService.DbContexts;
using TaskerService.Entities;
using TaskerService.Services;

namespace TaskerService.Consumers
{
    public class BookingStatusChangedConsumer : IConsumer<BookingStatusChangedEvent>
    {
        private readonly ITaskerAvailabilityService _availabilityService;
        private readonly ILogger<BookingStatusChangedConsumer> _logger;


        public BookingStatusChangedConsumer(
            ILogger<BookingStatusChangedConsumer> logger, ITaskerAvailabilityService availabilityService)
        {
            _logger = logger;
            _availabilityService = availabilityService;
        }

        public async Task Consume(ConsumeContext<BookingStatusChangedEvent> context)
        {
            var message = context.Message;
            try
            {
                _logger.LogInformation("Processing BookingStatusChangedEvent for booking {BookingId}", message.BookingId);

                await _availabilityService.RemoveUnavailabilitySlotAsync(message.BookingId);

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
