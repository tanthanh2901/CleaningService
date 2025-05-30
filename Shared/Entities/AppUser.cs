using Microsoft.AspNetCore.Identity;

namespace Shared.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public string FullName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string Avatar { get; set; } = string.Empty;
    }
}
