using AutoMapper;
using EventBus;
using MessageBus.IntegrationEvents;
using Microsoft.AspNetCore.Identity;
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

        public async Task PromoteToTaskerAsync(int userId, List<int> categoryIds)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogInformation("User not found.");
                return;
            }

            if (await userManager.IsInRoleAsync(user, "tasker"))
            {
                return;
            }
            else 
                await userManager.AddToRoleAsync(user, "tasker");

            if (user.PhoneNumber is null)
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
        }

        public async Task<bool> UpdateUserInfoAsync(int userId, UpdateUserInfoViewModel model)
        {
            var appUser = await userRepository.GetUserById(userId);

            if (appUser == null)
            {
                return false;
            }

            var userToUpdate = mapper.Map<AppUser>(model);

            await userRepository.UpdateUserInfo(userToUpdate);

            return true;
        }
    }
}
