using AuthService.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Shared.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.JwtExtensions
{
    public class JwtProvider : IJwtProvider
    {

        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> userManager;

        public JwtProvider(IConfiguration configuration, UserManager<AppUser> userManager)
        {
            _configuration = configuration;
            this.userManager = userManager;
        }

        public async Task<TokenDto> Generate(AppUser user)
        {
            var roles = await userManager.GetRolesAsync(user);

            var securityKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var signingCredentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("username", user.UserName.ToString()),

                new Claim("userId", user.Id.ToString()),
            };

            foreach (var role in roles)
            {
                claimsForToken.Add(new Claim(ClaimTypes.Role, role));  // or use "role" as the claim type
            }

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["JwtSettings:Issuer"],
                _configuration["JwtSettings:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);

            var accessToken = new JwtSecurityTokenHandler()
               .WriteToken(jwtSecurityToken);

            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await userManager.UpdateAsync(user);

            return new TokenDto(accessToken, refreshToken);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        private ClaimsPrincipal GetClaimsPrincipalFromExpireToken(string token)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretForKey"]);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidAudience = _configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken is null || !jwtSecurityToken.Header.Alg
                .Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityException("Invalid token");
            }

            return principal;
        }

        public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
        {
            var principal = GetClaimsPrincipalFromExpireToken(tokenDto.accessToken);
            var userName = principal.Claims.FirstOrDefault(c => c.Type == "username")?.Value;
            var user = await userManager.FindByNameAsync(userName);
            if (user == null || user.RefreshToken != tokenDto.refreshToken ||
                user.RefreshTokenExpiryTime < DateTime.Now)
            {
                throw new Exception("Invalid or expired refresh token.");
            }

            return await Generate(user);
        }
    }
}
