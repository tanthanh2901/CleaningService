namespace AuthService.JwtExtensions
{
    public interface IJwtProvider
    {
        string GenerateToken(int userId, string email);
    }
}
