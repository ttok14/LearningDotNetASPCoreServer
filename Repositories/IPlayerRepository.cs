using JNetwork;
using LearningServer01.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace LearningServer01.Repositories
{
    public interface IPlayerRepository
    {
        Task<bool> SaveChangesAsync(
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0);

        Task<DbTransaction> BeginTransactionAsync();

        Task<PlayerInfo> GetPlayerAsync(string id, E_PlayerInclude includes = E_PlayerInclude.None, bool isReadonly = false);

        void AddPlayer(PlayerInfo info);
        Task<bool> IsPlayerExistByIDAsync(string id);
        Task<(bool res, string nickname)> IsPlayerExistAndGetNicknameByIDAsync(string id);
        Task<bool> IsPlayerExistByNickname(string nickname);

        Task<(ERROR_CODE errCode, long uid, int remainedGold, int remainedWood, int remainedFood)> CreateStructure(string userId, int tableId, float positionX, float positionZ, float rotationY, GameDB.E_CurrencyType costType, int costPrice);

        void RemoveEntity(EntityItemInfo entity);

        Task<(PlayerInfo? opponent, bool foundInExcluede)> GetRandomOpponentAsync(string askerId, IReadOnlyList<string> excludeIds);

        BattleLogInfo AddBattleLog(
            string sessionId,
            string attackerId,
            string attackerNickname,
            string defenderId,
            string defenderNickname,
            DateTime timeUtc,
            S_BattleResult result,
            int lootedGold,
            int lootedWood,
            int lootedFood,
            S_BattleModeType modeType);

        Task<BattleLogInfo> GetBattleLogAsync(long logUid);
    }
}
