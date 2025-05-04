using OrderService.Dtos;
using OrderService.Entities;
using OrderService.Models;

namespace OrderService.Interface
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrder(Order order);
        Task<IEnumerable<OrderDto>> GetAllOrders(int userId);
        Task<OrderDto> GetOrderByIdAsync(int orderId);
        Task<bool> UpdateOrderStatusAsync(int orderId, BookingStatus payStatus);
        Task<IEnumerable<OrderDto>> GetOrderByStatus(int userId, BookingStatus payStatus);
        Task<OrderDto> Checkout(CheckoutRequest checkoutRequest);
    }
}
