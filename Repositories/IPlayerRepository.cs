namespace LearningServer01.Repositories
{
    public interface IPlayerRepository
    {
        Task<PlayerInfo> GetPlayerAsync(string id);

        Task<bool> AddPlayerAsync(PlayerInfo info);

        Task<bool> AddGold(string userId, int amount);
        Task<bool> AddWood(string userId, int amount);
        Task<bool> AddFood(string userId, int amount);
    }
}
