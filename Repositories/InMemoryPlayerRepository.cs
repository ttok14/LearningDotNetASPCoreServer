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

        public async Task<PlayerInfo> GetPlayer(string nickname)
        {
            await Task.Delay(1000);

            _db.TryGetValue(nickname, out var info);
            if (info == null)
                return null;
            return info;
        }

        public async Task<bool> AddPlayer(PlayerInfo info)
        {
            await Task.Delay(1000);

            if (_db.ContainsKey(info.NickName))
                return false;

            _db.Add(info.NickName, info);

            return true;
        }
    }
}
