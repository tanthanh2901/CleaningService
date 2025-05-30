using OrderService.Enums;

namespace OrderService.Models
{
    public class UpdateBookingStatusModel
    {
        public int BookingId { get; set; }
        public BookingStatus? BookingStatus { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
    }
}
