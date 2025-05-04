using Microsoft.AspNetCore.Identity;

namespace AuthService.Models
{
    public class AppUser : IdentityUser<int>
    {
        public string FullName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
