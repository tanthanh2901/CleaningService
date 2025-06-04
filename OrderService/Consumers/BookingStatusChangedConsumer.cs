using MassTransit;
using MessageBus.IntegrationEvents;
using OrderService.Enums;
using OrderService.Interface;

namespace OrderService.Consumers
{
    public class BookingStatusChangedConsumer : IConsumer<BookingStatusChangedEvent>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ILogger<BookingStatusChangedConsumer> _logger;

        public BookingStatusChangedConsumer(
            IBookingRepository bookingRepository,
            ILogger<BookingStatusChangedConsumer> logger)
        {
            _bookingRepository = bookingRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<BookingStatusChangedEvent> context)
        {
            var message = context.Message;
            try
            {
                _logger.LogInformation("Processing BookingStatusChangedEvent for booking {BookingId}", message.BookingId);

                var booking = await _bookingRepository.GetBookingByIdAsync(message.BookingId);
                
                if (booking == null)
                {
                    _logger.LogWarning("Booking {BookingId} not found", message.BookingId);
                    return;
                }

                var bookingStatus = message.NewStatus.ToLower();
                var statusToUpdate = BookingStatus.Assigned;

                switch (bookingStatus)
                {
                    case "confirmed":
                        statusToUpdate = BookingStatus.Confirmed;
                        break;
                    case "completed":
                        statusToUpdate = BookingStatus.Completed;
                        break;
                    case "canceled":
                        statusToUpdate = BookingStatus.Canceled;
                        break;
                    case "inprogress":
                        statusToUpdate = BookingStatus.InProgress;
                        break;
                    default:
                        _logger.LogWarning("Unknown booking status: {Status}", message.NewStatus);
                        return;
                }

                await _bookingRepository.UpdateBookingStatusAsync(message.BookingId, statusToUpdate, null);
                _logger.LogInformation("Successfully updated booking {BookingId} status to {Status}", 
                    message.BookingId, statusToUpdate);
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
