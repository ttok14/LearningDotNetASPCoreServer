using JNetwork;
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

        Task<PlayerInfo> GetPlayerBasicAsync(string id, bool isReadonly = false);
        Task<PlayerInfo> GetPlayerFullAsync(string id, bool isReadonly = false);

        void AddPlayer(PlayerInfo info);
        Task<bool> IsPlayerExistAsync(string id);

        Task<(ERROR_CODE errCode, long uid, int remainedGold, int remainedWood, int remainedFood)> CreateStructure(string userId, int tableId, float positionX, float positionZ, float rotationY, GameDB.E_CurrencyType costType, int costPrice);
        Task<bool> DestroyStructure(string userId, long uid);

        Task<PlayerInfo> GetRandomOpponentAsync(string excludeUserId);
    }
}
