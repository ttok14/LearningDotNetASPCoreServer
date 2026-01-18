namespace LearningServer01.Repositories
{
    public interface IPlayerRepository
    {
        Task<PlayerInfo> GetPlayerAsync(string id);

        Task<bool> AddPlayerAsync(PlayerInfo info);
    }
}
