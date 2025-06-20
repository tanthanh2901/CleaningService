using Shared.Enums;

namespace TaskerService.Models
{
    public class UpdateBookingStatusModel
    {
        public int BookingId { get; set; }
        public BookingStatus BookingStatus { get; set; }
    }
}
