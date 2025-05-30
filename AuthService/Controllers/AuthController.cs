using System.Security.Claims;
using AuthService.Dto;
using AuthService.JwtExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared.Entities;

namespace AuthService.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        private readonly IJwtProvider jwtProvider;
        public AuthController(UserManager<AppUser> userManager, IJwtProvider jwtProvider)
        {
            _userManager = userManager;
            this.jwtProvider = jwtProvider;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var user = new AppUser { UserName = model.Email, Email = model.Email, FullName = model.FullName };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized("Invalid credentials");

            var tokenDto = await jwtProvider.Generate(user);

            Response.Cookies.Append("accessToken", tokenDto.accessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.Now.AddHours(1),
                Secure = true,
                SameSite = SameSiteMode.None,
            });

            Response.Cookies.Append("refreshToken", tokenDto.refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.Now.AddDays(7),
                Secure = true,
                SameSite = SameSiteMode.None,
            });

            return Ok(new { accessToken = tokenDto.accessToken });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userIdString);

            if (user != null)
            {
                user.RefreshToken = null;
                await _userManager.UpdateAsync(user);
            }

            Response.Cookies.Delete("accessToken");
            Response.Cookies.Delete("refreshToken");

            return Ok(new { message = "Logged out successfully" });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenDto tokenDto)
        {
            var newTokens = await jwtProvider.RefreshToken(tokenDto);

            Response.Cookies.Append("accessToken", newTokens.accessToken,
            new CookieOptions
            {
               HttpOnly = true,
               Expires = DateTimeOffset.Now.AddHours(1),
               Secure = true,
               SameSite = SameSiteMode.None,
            });

            Response.Cookies.Append("refreshToken", newTokens.refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.Now.AddDays(7),
                Secure = true,
                SameSite = SameSiteMode.None,
            });
            return Ok(newTokens);
        }
    }
}
