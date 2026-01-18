namespace LearningServer01.Repositories
{
    public class InMemoryPlayerRepository : IPlayerRepository
    {
        Dictionary<string, PlayerInfo> _db = new Dictionary<string, PlayerInfo>()
        {
            ["YunSeon"] = new PlayerInfo()
            {
                ID = "ttok14",
                Level = 51,
                Gold = 1000,
            }
        };

        public async Task<PlayerInfo> GetPlayerAsync(string nickname)
        {
            await Task.Delay(1000);

            _db.TryGetValue(nickname, out var info);
            if (info == null)
                return null;
            return info;
        }

        public async Task<bool> AddPlayerAsync(PlayerInfo info)
        {
            await Task.Delay(1000);

            if (_db.ContainsKey(info.ID))
                return false;

            _db.Add(info.ID, info);

            return true;
        }
    }
}
