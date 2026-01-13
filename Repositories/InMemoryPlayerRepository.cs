namespace LearningServer01.Repositories
{
    public class InMemoryPlayerRepository : IPlayerRepository
    {
        Dictionary<string, PlayerInfo> _db = new Dictionary<string, PlayerInfo>()
        {
            ["YunSeon"] = new PlayerInfo()
            {
                NickName = "ttok14",
                Level = 51,
                Items = new List<string>() { "NinjaSword01", "Shield01" }
            }
        };

        public PlayerInfo? GetPlayer(string nickname)
        {
            _db.TryGetValue(nickname, out var info);
            if (info == null)
                return null;
            return info;
        }

        public bool AddPlayer(PlayerInfo info)
        {
            if (_db.ContainsKey(info.NickName))
                return false;

            _db.Add(info.NickName, info);

            return true;
        }
    }
}
