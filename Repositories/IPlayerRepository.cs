namespace LearningServer01.Repositories
{
    public interface IPlayerRepository
    {
        Task<PlayerInfo> GetPlayer(string nickName);

        Task<bool> AddPlayer(PlayerInfo info);
    }
}
