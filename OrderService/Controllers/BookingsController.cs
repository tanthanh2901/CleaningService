using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Dtos;
using OrderService.Interface;
using OrderService.Models;
using Shared.Enums;

namespace OrderService.Controllers
{
    [Route("api/orders")]
    [ApiController]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingRepository orderRepository;

        public BookingsController(IBookingRepository orderRepository)
        {
            this.orderRepository = orderRepository;
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


        [HttpGet("my-orders")]
        public async Task<ActionResult<List<BookingDto>>> GetBookings()
        {
            var userId = await GetUserId();
            if (userId == null) return Unauthorized();

            var orders = await orderRepository.GetBookings(userId);
            return Ok(orders);
        }

        [HttpGet("all")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<BookingDto>>> GetAllBookings()
        {
            var orders = await orderRepository.GetAllBookings();
            return Ok(orders);
        }

        [HttpGet("tasker/{taskerId}")]
        [Authorize(Roles = "tasker")]
        public async Task<ActionResult<List<BookingDto>>> GetBookingsByTasker(int taskerId)
        {
            var orders = await orderRepository.GetBookingsByTaskerId(taskerId);
            return Ok(orders);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            var order = await orderRepository.GetBookingByIdAsync(orderId);

            return Ok(order);
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetBookingByStatus(BookingStatus status)
        {
            var userId = await GetUserId();
            if (userId == null) return Unauthorized();

            var orders = await orderRepository.GetBookingByStatus(userId, status);

            return Ok(orders);
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout(CheckoutRequest checkoutRequest)
        {
            var orderDto = await orderRepository.Checkout(checkoutRequest);

            return Ok(orderDto);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("update-status")]
        [Authorize(Roles = "tasker")]
        public async Task<ActionResult<bool>> UpdateBookingStatus([FromBody] UpdateBookingStatusModel dto)
        {
            var result = await orderRepository.UpdateBookingStatusAsync(dto.BookingId, dto.BookingStatus, dto.PaymentStatus);
            if (!result) return NotFound();
            return Ok(true);
        }

        [HttpPut("{orderId}")]
        public async Task<ActionResult<bool>> CancelBooking(int orderId)
        {
            var result = await orderRepository.CancelBooking(orderId);
            if (!result) return NotFound();
            return Ok(true);
        }
    }
}
