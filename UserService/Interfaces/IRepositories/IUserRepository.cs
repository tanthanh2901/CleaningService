using Microsoft.AspNetCore.Identity;
using Shared.Entities;

namespace UserService.Interfaces.IRepositories
{
    public interface IUserRepository
    {
        Task<AppUser> GetUserById(int userId);
        Task<List<AppUser>> GetAllUsers();
        Task UpdateUserInfo(AppUser user);
        Task UpdateUserAvatar(int userId, string avatarUrl);
        Task<IdentityResult> DeleteUser(AppUser user);
    }
}
