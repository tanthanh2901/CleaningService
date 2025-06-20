using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TaskerService.Services;
using TaskerService.Entities;
using Microsoft.AspNetCore.Authorization;
using TaskerService.Models;

namespace TaskerService.Controllers
{
    [Route("api/taskers")]
    [ApiController]
    public class TaskersController : ControllerBase
    {
        private readonly ITaskerService taskerService;
        private readonly IBookingService bookingService;
        private readonly ITaskerAvailabilityService _availabilityService;

        public TaskersController(ITaskerService taskerService, IBookingService bookingService, ITaskerAvailabilityService availabilityService)
        {
            this.taskerService = taskerService;
            this.bookingService = bookingService;
            _availabilityService = availabilityService;
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

        [HttpGet("available-by-category/{categoryId}")]
        public async Task<IActionResult> GetAvailableTaskersByCategory(
        int categoryId,
        [FromQuery] DateTime startTime,
        [FromQuery] DateTime endTime)
        {
            try
            {
                var taskers = await _availabilityService.GetAvailableTaskersByCategoryAsync(
                    categoryId, startTime, endTime);
                return Ok(taskers);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
        public async Task<IActionResult> GetTaskerBookings()
        {
            var userId = await GetUserId();
            if (userId == null)
            {
                return NotFound("User not found");
            }

            var tasker = await taskerService.GetTaskerByUserId(userId);
            if (tasker == null) return NotFound("Tasker not found.");

            int taskerId = tasker.TaskerId;

            var bookings = await bookingService.GetBookingsByTaskerId(taskerId);
            return Ok(bookings);
        }

        [Authorize(Roles = "tasker")]
        [HttpGet("bookings/{bookingId}")]
        public async Task<IActionResult> GetTaskerBooking(int bookingId)
        {
            //var bookings = await taskerService.GetTaskerBookings(taskerId);
            //return Ok(bookings);

            var booking = await bookingService.GetBookingByIdAsync(bookingId);
            return Ok(booking);
        }

        [Authorize(Roles = "tasker")]
        [HttpPost("bookings/update-status")]
        public async Task<IActionResult> UpdateBookingStatus(UpdateBookingStatusModel model)
        {
            var result = await taskerService.UpdateBookingStatus(model);
            if (!result)
                return NotFound("Booking not found");

            return Ok();
        }

        //[Authorize(Roles = "tasker")]
        //[HttpPost("bookings/{bookingId}/confirm")]
        //public async Task<IActionResult> ConfirmBooking(int bookingId)
        //{
        //    var result = await taskerService.ConfirmBooking(bookingId);
        //    if (!result)
        //        return NotFound("Booking not found");

        //    return Ok();
        //}

        //[Authorize(Roles = "tasker")]
        //[HttpPost("bookings/{bookingId}/start")]
        //public async Task<IActionResult> StartBooking(int bookingId)
        //{
        //    var result = await taskerService.StartBooking(bookingId);
        //    if (!result)
        //        return NotFound("Booking not found");

        //    return Ok();
        //}

        //[Authorize(Roles = "tasker")]
        //[HttpPost("bookings/{bookingId}/complete")]
        //public async Task<IActionResult> CompleteBooking(int bookingId)
        //{
        //    var result = await taskerService.CompletedBooking(bookingId);
        //    if (!result)
        //        return NotFound("Booking not found");

        //    return Ok();
        //}
    }
}
