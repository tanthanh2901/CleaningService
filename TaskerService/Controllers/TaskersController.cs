using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TaskerService.Services;
using TaskerService.Entities;
using Microsoft.AspNetCore.Authorization;

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

        [HttpGet("by-taskerId/{taskerId}")]
        public async Task<IActionResult> GetTaskerByTaskerId(int taskerId)
        {
            var result = await taskerService.GetTaskerByTaskerId(taskerId);
            return Ok(result);
        }

        [HttpGet("by-userId/{userId}")]
        public async Task<IActionResult> GetTaskerByUserId(int userId)
        {
            var result = await taskerService.GetTaskerByUserId(userId);
            return Ok(result);
        }

        [HttpGet("get-available/{categoryId}")]
        public async Task<IActionResult> GetTaskerFree(int categoryId)
        {
            var result = await taskerService.GetAvailableTaskersByCategory(categoryId);
            return Ok(result);
        }

        [Authorize(Roles = "tasker")]
        [HttpPut("set-available")]
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

        [Authorize(Roles = "tasker")]
        [HttpGet("bookings")]
        public async Task<IActionResult> GetTaskerBookings(int taskerId)
        {
            var bookings = await taskerService.GetTaskerBookings(taskerId);
            return Ok(bookings);
        }

        [Authorize(Roles = "tasker")]
        [HttpPost("bookings/{bookingId}/confirm")]
        public async Task<IActionResult> ConfirmBooking(int bookingId)
        {
            var result = await taskerService.ConfirmBooking(bookingId);
            if (!result)
                return NotFound("Booking not found");

            return Ok();
        }

        [Authorize(Roles = "tasker")]
        [HttpPost("bookings/{bookingId}/start")]
        public async Task<IActionResult> StartBooking(int bookingId)
        {
            var result = await taskerService.StartBooking(bookingId);
            if (!result)
                return NotFound("Booking not found");

            return Ok();
        }

        [Authorize(Roles = "tasker")]
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
