using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.DbContexts;
using Shared.Entities;
using UserService.Interfaces.IRepositories;

namespace UserService.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppUser> userManager;
        private readonly ILogger<UserRepository> _logger;
        private readonly UserDbContext dbContext;

        public UserRepository(UserManager<AppUser> userManager, ILogger<UserRepository> logger, UserDbContext dbContext)
        {
            this.userManager = userManager;
            _logger = logger;
            this.dbContext = dbContext;
        }

        public async Task UpdateUserAvatar(int userId, string avatarUrl)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                user.Avatar = avatarUrl;
                await dbContext.SaveChangesAsync();
            }
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
