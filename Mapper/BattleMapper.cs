using JNetwork;

namespace LearningServer01
{
    public static class BattleMapper
    {
        public static BattleLogNetData ToBattleLogNetData(this BattleLogInfo logInfo)
        {
            return new BattleLogNetData()
            {
                SessionId = logInfo.SessionId,
                ID = logInfo.ID,
                DefenderId = logInfo.DefenderId,
                DefenderNickname = logInfo.DefenderNickname,
                AttackerId = logInfo.AttackerId,
                AttackerNickname = logInfo.AttackerNickname,
                BattleResult = logInfo.BattleResult,
                LootedGold = logInfo.LootedGold,
                LootedWood = logInfo.LootedWood,
                LootedFood = logInfo.LootedFood,
                BattleMode = logInfo.ModeType,
                IsRevenged = logInfo.IsRevenged,
                LogTimeUtcTicks = logInfo.LogTimeUtc.Ticks
            };
        }
    }
}
