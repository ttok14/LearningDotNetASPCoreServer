namespace LearningServer01.Repositories
{
    public interface IPlayerRepository
    {
        PlayerInfo GetPlayer(string nickName);

        bool AddPlayer(PlayerInfo info);
    }
}
