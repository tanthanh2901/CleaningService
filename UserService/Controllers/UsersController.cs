using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared.Entities;
using UserService.Interfaces.IRepositories;
using UserService.Interfaces.IServices;
using UserService.Models;
using UserService.Services;

namespace UserService.Controllers
{
    [Authorize]
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IUserService userService;

        public UsersController(IUserService userService, UserManager<AppUser> userManager)
        {
            this.userService = userService;
            this.userManager = userManager;
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

        [HttpGet("me")]
        public async Task<IActionResult> GetUserInfo()
        {
            var userId = await GetUserId();
            if (userId == null)
            {
                return NotFound("User not found");
            }

            var userInfo = await userService.GetUserInfoAsync(userId);
            return Ok(userInfo);
        }

        [HttpGet("{userId}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var user = await userService.GetUserInfoAsync(userId);
            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await userService.GetAllUsersAsync();
            return Ok(users);
        }

       
        [HttpPut("me")]
        public async Task<IActionResult> UpdateUserInfo(UpdateUserInfoViewModel model)
        {
            var userId = await GetUserId();

            await userService.UpdateUserInfoAsync(userId, model);

            return NoContent();
        }

        [HttpPost("upload-avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile avatar)
        {
            try
            {
                var userId = await GetUserId();

                var avatarPath = await userService.UploadAvatarAsync(userId, avatar);
                return Ok(new { url = avatarPath });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{userId}")]
        public async Task<IActionResult> AdminUpdateUserInfo(int userId, UpdateUserInfoViewModel model)
        {
            await userService.UpdateUserInfoAsync(userId, model);

            return NoContent();
        }

        [HttpDelete("{userId}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var result = await userService.DeleteUserAsync(userId);
            if (!result) return NotFound();

            return NoContent();
        }

        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var userId = await GetUserId();

            await userService.ChangePasswordAsync(userId, model);

            return Ok();
        }

        [Authorize(Roles = "admin")]
        [HttpPut("promote-to-tasker")]
        public async Task<IActionResult> PromoteToTasker([FromBody] PromoteToTaskerRequest request)
        {
            await userService.PromoteToTaskerAsync(request.UserId, request.CategoryIds);

            return Ok();
        }

        [Authorize(Roles = "admin")]
        [HttpPost("tasker")]
        public async Task<IActionResult> CreateTasker([FromBody] CreateTaskerRequest request)
        {
            var result = await userService.CreateUserAsync(new CreateUserRequest
            {
                FullName = request.FullName,
                UserName = request.UserName,
                Email = request.Email,
                Password = request.Password,
                PhoneNumber = request.PhoneNumber,
                Role = "tasker"
            });

            if (result.Succeeded)
            {
                var user = await userManager.FindByEmailAsync(request.Email);
                await userService.PromoteToTaskerAsync(user.Id, request.CategoryIds);
                return Ok("Tasker created");
            }

            return BadRequest(result.Errors);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("customer")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var result = await userService.CreateUserAsync(request);
            if (result.Succeeded)
            {
                return Ok(new { message = "User created successfully" });
            }

            return BadRequest(result.Errors);
        }
    }
}
