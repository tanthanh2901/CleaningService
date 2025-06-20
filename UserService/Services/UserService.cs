using System.Data;
using AutoMapper;
using EventBus;
using MessageBus.IntegrationEvents;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Entities;
using UserService.Dtos;
using UserService.Interfaces.IRepositories;
using UserService.Interfaces.IServices;
using UserService.Models;

namespace UserService.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<AppRole> roleManager;
        private readonly ILogger<UserService> _logger;
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly IEventBus eventBus;

        public UserService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, ILogger<UserService> logger, IUserRepository userRepository, IMapper mapper, IEventBus eventBus)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _logger = logger;
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.eventBus = eventBus;
        }

        public async Task<bool> AssignRoleAsync(int userId, string role)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogInformation("User not found.");
                return false;
            }

            var result = await userManager.AddToRoleAsync(user, role);
            if (result.Succeeded)
            {
                _logger.LogInformation($"Role '{role}' assigned to user {user.UserName}.");
                return true;
            }
            else
            {
                _logger.LogError($"Failed to assign role '{role}' to user {user.UserName}.");
                return false;
            }
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordViewModel model)
        {
            var appUser = await userManager.FindByIdAsync(userId.ToString());

            if (appUser == null)
            {
                throw new Exception("User not found");
            }

            var result = await userManager.ChangePasswordAsync(appUser, model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                _logger.LogInformation($"Password changed successfully for user: {appUser.UserName}");

                return true; // Password change was successful
            }

            // Handle failure
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));

                _logger.LogError($"Failed to change password for user: {appUser.UserName}. Errors: {errors}");

                throw new Exception($"Failed to change password: {errors}");
            }
        }

        public async Task<IdentityResult> CreateUserAsync(CreateUserRequest request)
        {
            var user = new AppUser
            {
                Email = request.Email,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                FullName = request.UserName
            };

            var result = await userManager.CreateAsync(user, request.Password);
            
            if (!result.Succeeded) return result;

            // Ensure role exists
            if (!await roleManager.RoleExistsAsync(request.Role))
            {
                await roleManager.CreateAsync(new AppRole { Name = request.Role });
            }

            await userManager.AddToRoleAsync(user, request.Role);

            return result;
        }
        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await userRepository.GetUserById(userId);
            if (user == null)
                return false;

            var result = await userRepository.DeleteUser(user);

            return result.Succeeded;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await userRepository.GetAllUsers();

            var userDtos = new List<UserDto>();

            foreach(var user in users)
            {
                var userDto = mapper.Map<UserDto>(user);

                var roles = await userManager.GetRolesAsync(user);

                userDto.Roles = roles.ToList();

                userDtos.Add(userDto);
            }

            return userDtos;
        }

        public async Task<UserDto?> GetUserInfoAsync(int userId)
        {
            var user = await userRepository.GetUserById(userId);

            if (user == null) return null;

            var userDto = mapper.Map<UserDto>(user);

            var roles = await userManager.GetRolesAsync(user);

            userDto.Roles = roles.ToList();
            return userDto;
        }

        public async Task<bool> PromoteToTaskerAsync(int userId, List<int> categoryIds)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogInformation("User not found.");
                return false;
            }

            if (await userManager.IsInRoleAsync(user, "tasker"))
            {
                return true;
            }
            
            var result = await userManager.AddToRoleAsync(user, "tasker");
            if(!result.Succeeded)
            {
                return false;
            }

            if (string.IsNullOrEmpty(user.PhoneNumber))
                user.PhoneNumber = "";

            var taskerEvent = new TaskerCreatedEvent
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                CategoryIds = categoryIds
            };

            await eventBus.PublishAsync(taskerEvent);

            return true;
        }

        public async Task<string> UploadAvatarAsync(int userId, IFormFile avatar)
        {
            if (avatar == null || avatar.Length == 0)
                throw new ArgumentException("No file provided");

            // Validate file type
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
            if (!allowedTypes.Contains(avatar.ContentType.ToLower()))
                throw new ArgumentException("Invalid file type. Only JPEG, PNG, and GIF are allowed");

            // Validate file size (e.g., max 5MB)
            const int maxSizeInBytes = 5 * 1024 * 1024;
            if (avatar.Length > maxSizeInBytes)
                throw new ArgumentException("File size exceeds maximum limit of 5MB");

            // Get existing user
            var user = await userRepository.GetUserById(userId);

            // Delete old avatar file if exists
            if (!string.IsNullOrEmpty(user.Avatar))
            {
                var oldFilePath = Path.Combine("wwwroot", user.Avatar.TrimStart('/'));
                if (File.Exists(oldFilePath))
                    File.Delete(oldFilePath);
            }

            // Generate unique filename
            var fileExtension = Path.GetExtension(avatar.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

            // Define upload path
            var uploadPath = Path.Combine("wwwroot", "uploads", "avatars");

            // Create directory if it doesn't exist
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, uniqueFileName);

            // Save file to disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await avatar.CopyToAsync(stream);
            }

            // Update user avatar path in database
            var avatarPath = $"/uploads/avatars/{uniqueFileName}";

            await userRepository.UpdateUserAvatar(userId, avatarPath);

            return avatarPath;
        }
        public async Task<bool> UpdateUserInfoAsync(int userId, UpdateUserInfoViewModel model)
        {
            var appUser = await userRepository.GetUserById(userId);

            if (appUser == null)
            {
                return false;
            }

            //var userToUpdate = mapper.Map<AppUser>(model);
            
            mapper.Map(model, appUser);
            await userRepository.UpdateUserInfo(appUser);

            var isTasker = await userManager.IsInRoleAsync(appUser, "tasker");
            if (isTasker)
            {
                var taskerUpdatedEvent = new TaskerInfoUpdatedEvent
                {
                    UserId = userId,
                    FullName = appUser.FullName,
                    Address = appUser.Address,
                    PhoneNumber = appUser.PhoneNumber,
                    Avatar = appUser.Avatar
                };

                await eventBus.PublishAsync(taskerUpdatedEvent);
            }
            return true;
        }

        
    }
}
