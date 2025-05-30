using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.DbContexts;
using Shared.Entities;
using UserService.Dtos;
using UserService.Interfaces.IRepositories;
using UserService.Models;

namespace UserService.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<AppRole> roleManager;
        private readonly ILogger<UserRepository> _logger;
        private readonly UserDbContext dbContext;

        public UserRepository(UserManager<AppUser> userManager, ILogger<UserRepository> logger, UserDbContext dbContext, RoleManager<AppRole> roleManager)
        {
            this.userManager = userManager;
            _logger = logger;
            this.dbContext = dbContext;
            this.roleManager = roleManager;
        }

        public async Task<IdentityResult> DeleteUser(AppUser user)
            => await userManager.DeleteAsync(user);

        public async Task<AppUser> GetUserById(int userId)
             => await userManager.FindByIdAsync(userId.ToString());

        public async Task<List<AppUser>> GetAllUsers()
        {
            var users = await userManager.Users.ToListAsync();
            if (users == null || users.Count == 0)
            {
                _logger.LogInformation("No users found");
            }

            return users;
        }

        public async Task UpdateUserInfo(AppUser user)
            => await userManager.UpdateAsync(user);
    }
}
