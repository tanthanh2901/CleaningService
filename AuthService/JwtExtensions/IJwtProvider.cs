using AuthService.Dto;
using Shared.Entities;

namespace AuthService.JwtExtensions
{
    public interface IJwtProvider
    {
        Task<TokenDto> Generate(AppUser user);
        Task<TokenDto> RefreshToken(TokenDto tokenDto);
    }
}
