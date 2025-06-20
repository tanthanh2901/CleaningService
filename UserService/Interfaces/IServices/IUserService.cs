using Microsoft.AspNetCore.Identity;
using Shared.Entities;
using UserService.Dtos;
using UserService.Models;

namespace UserService.Interfaces.IServices
{
    public interface IUserService
    { 
        Task<UserDto?> GetUserInfoAsync(int userId);
        //Task<AppUser> GetFullUserInfoAsync(int userId);
        Task<bool> UpdateUserInfoAsync(int userId, UpdateUserInfoViewModel model);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordViewModel model);
        Task<string> UploadAvatarAsync(int userId, IFormFile avatar);

        //Task<AppUser> AdminGetUserInfo(int userId);

        Task<IdentityResult> CreateUserAsync(CreateUserRequest createUserRequest);
        Task<bool> AssignRoleAsync(int userId, string role);
        Task<bool> PromoteToTaskerAsync(int userId, List<int> categoryIds);

        Task<List<UserDto>> GetAllUsersAsync();
        Task<bool> DeleteUserAsync(int userId);
    }
}
