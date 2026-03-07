using JNetwork;

namespace LearningServer01.Services.AuthService
{
    public interface IAuthService
    {
        Task<(ERROR_CODE errCode, string token, PlayerInfo? loggedInPlayerInfo)> LoginAsync(string id, string userPassword);
        string CreateToken(string userId);
        bool VerifyPassword(string rawPassword, string hashedPassword);
    }
}
