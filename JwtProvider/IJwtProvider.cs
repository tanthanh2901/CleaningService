namespace JwtConfiguration
{
    public interface IJwtProvider
    {
        string GenerateToken(int userId, string email);
    }
}
