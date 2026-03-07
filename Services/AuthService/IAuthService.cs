namespace LearningServer01.Services.AuthService
{
    public interface IAuthService
    {
        string CreateToken(string userId);
        bool VerifyPassword(string rawPassword, string hashedPassword);
    }
}
