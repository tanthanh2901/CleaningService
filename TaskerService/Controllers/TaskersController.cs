using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TaskerService.Services;
using TaskerService.Entities;

namespace TaskerService.Controllers
{
    [Route("api/taskers")]
    [ApiController]
    public class TaskersController : ControllerBase
    {
        private readonly ITaskerService taskerService;

        public TaskersController(ITaskerService taskerService)
        {
            this.taskerService = taskerService;
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
        public async Task<IActionResult> GetTaskerFree(int categoryId)
        {
            var result = await taskerService.GetAvailableTaskersByCategory(categoryId);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> SetAvailable()
        {
            var userId = await GetUserId();
            if (userId == null)
            {
                return NotFound("User not found");
            }

            var result = await taskerService.SetAvailable(userId);
            return Ok(result);
        }

        [HttpGet("bookings")]
        public async Task<IActionResult> GetTaskerBookings(int taskerId, string? status = null)
        {
            var bookings = await taskerService.GetTaskerBookings(taskerId, status);
            return Ok(bookings);
        }

        [HttpPost("bookings/{bookingId}/confirm")]
        public async Task<IActionResult> ConfirmBooking(int bookingId)
        {
            var result = await taskerService.ConfirmBooking(bookingId);
            if (!result)
                return NotFound("Booking not found");

            return Ok();
        }


        [HttpPost("bookings/{bookingId}/complete")]
        public async Task<IActionResult> CompleteBooking(int bookingId)
        {
            var result = await taskerService.CompletedBooking(bookingId);
            if (!result)
                return NotFound("Booking not found");

            return Ok();
        }
    }
}
