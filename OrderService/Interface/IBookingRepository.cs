using OrderService.Dtos;
using OrderService.Models;
using Shared.Enums;

namespace OrderService.Interface
{
    public interface IBookingRepository
    {
        Task<IEnumerable<BookingDto>> GetAllBookings();
        Task<IEnumerable<BookingDto>> GetBookings(int userId);
        Task<IEnumerable<BookingDto>> GetBookingsByTaskerId(int taskerId);
        Task<BookingDetailsDto> GetBookingByIdAsync(int orderId);
        Task<bool> UpdateBookingStatusAsync(int orderId, BookingStatus? bookingStatus, PaymentStatus? paymentStatus, DateTime? updatedAt = null);
        Task<IEnumerable<BookingDto>> GetBookingByStatus(int userId, BookingStatus bookingStatus);
        Task<BookingDto> Checkout(CheckoutRequest checkoutRequest);
        Task<bool> CancelBooking(int orderId);
    }
}
