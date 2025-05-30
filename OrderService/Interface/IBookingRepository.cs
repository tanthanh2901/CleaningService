using OrderService.Dtos;
using OrderService.Enums;
using OrderService.Models;

namespace OrderService.Interface
{
    public interface IBookingRepository
    {
        Task<IEnumerable<BookingDto>> GetAllBookings();
        Task<IEnumerable<BookingDto>> GetBookings(int userId);
        Task<BookingDto> GetBookingByIdAsync(int orderId);
        Task<bool> UpdateBookingStatusAsync(int orderId, BookingStatus? bookingStatus, PaymentStatus? paymentStatus);
        Task<IEnumerable<BookingDto>> GetBookingByStatus(int userId, BookingStatus bookingStatus);
        Task<BookingDto> Checkout(CheckoutRequest checkoutRequest);
        Task<bool> CancelBooking(int orderId);
    }
}
