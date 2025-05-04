using System.Security.Claims;
using EventBus;
using MessageBus.IntegrationEvents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Dtos;
using OrderService.Entities;
using OrderService.Interface;
using OrderService.Models;

namespace OrderService.Controllers
{
    [Route("api/orders")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository orderRepository;
        private readonly IEventBus eventBus;

        public OrdersController(IOrderRepository orderRepository, IEventBus eventBus)
        {
            this.orderRepository = orderRepository;
            this.eventBus = eventBus;
        }

        private async Task<int> GetUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(userIdString, out var userId))
            {
                return userId;
            }

            // Handle case where userId cannot be parsed into an integer
            throw new InvalidOperationException("User ID is not valid.");
        }


        [HttpGet]
        public async Task<ActionResult<List<OrderDto>>> GetAllOrders()
        {
            var userId = await GetUserId();
            if (userId == null) return Unauthorized();

            var orders = await orderRepository.GetAllOrders(userId);
            return Ok(orders);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            var order = await orderRepository.GetOrderByIdAsync(orderId);

            return Ok(order);
        }

        [HttpGet("GetOrderByStatus")]
        public async Task<IActionResult> GetOrderByStatus(BookingStatus status)
        {
            var userId = await GetUserId();
            if (userId == null) return Unauthorized();

            var orders = await orderRepository.GetOrderByStatus(userId, status);

            return Ok(orders);
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout(CheckoutRequest checkoutRequest)
        {
            var orderDto = await orderRepository.Checkout(checkoutRequest);

            await eventBus.PublishAsync(
                new OrderCreatedEvent
                {
                    OrderId = orderDto.OrderId,
                    UserId = orderDto.UserId,
                    TotalAmount = orderDto.TotalAmount,
                    PaymentMethod = orderDto.PaymentMethod
                }
                );

            return Ok(orderDto);
        }

        [HttpPost("orders/{orderId}")]
        public async Task<ActionResult<bool>> UpdateOrderStatus(int orderId, BookingStatus status)
        {
            var update = await orderRepository.UpdateOrderStatusAsync(orderId, status);
            return Ok(update);
        }
    }
}
