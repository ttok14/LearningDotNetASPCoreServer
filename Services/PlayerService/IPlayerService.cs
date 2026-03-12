using GameDB;
using JNetwork;
using MessagePack.Formatters;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace LearningServer01
{
    public interface IPlayerService
    {
        Task<ERROR_CODE> PostLoginAsync(PlayerInfo? loggedInPlayerInfo);
        Task<(ERROR_CODE errCode, PlayerInfo? newUser)> RegisterNewPlayerAsync(string id, string password);

        Task<ERROR_CODE> CleanZombieBattleSessions(string attackerId);

        Task<ERROR_CODE> EnterNicknameAsync(string id, string nickname);
        Task<ERROR_CODE> SetStatusMessageAsync(string id, string message);

        Task<(ERROR_CODE errCode, PlayerInfo? myInfo)> EnterHomeAsync(string id);
        Task<(ERROR_CODE errCode, PlayerInfo? myInfo, PlayerInfo? opponentInfo)> SearchOpponentAsync(string id);
        Task<(ERROR_CODE errCode, PlayerInfo? myInfo, PlayerInfo? opponentInfo)> LoadRevengeAsync(string id, long battleLogUid, string opponentId);

#if DEBUG
        Task<(ERROR_CODE errCode, int remainedCurrency)> CheatCurrency(string id, E_CurrencyType currencyType);
#endif

        Task<ERROR_CODE> EquipDeploymentSlotAsync(string id, int equipSlotIdx, long equipItemUid);
        Task<ERROR_CODE> UnequipDeploymentSlotAsync(string id, int equipSlotIdx);

        Task<ERROR_CODE> GarrisonUnitAsync(string id, long ownerEntityUid, int slotIdx, long garrisonUnitUid);
        Task<ERROR_CODE> SetSpawnUnitAsync(string id, long ownerEntityUid, long spawnUnitUid);
        Task<(ERROR_CODE errCode, S_GarrisonSlotType resSlotType)> UngarrisonUnitAsync(string id, long ownerEntityUid, long ungarrisonUnitUid);

        Task<(ERROR_CODE errCode, long uid, int remainedGold, int remainedWood, int remainedFood)> BuyItemAsync(string id, int itemTid);
        Task<ERROR_CODE> MoveEntityAsync(string id, long entityUid, float posX, float posZ, float rotY);

        Task<ERROR_CODE> EquipHeroAsync(string id, long heroItemUid);
        Task<ERROR_CODE> UnequipHeroAsync(string id);

        Task<(ERROR_CODE errCode, long uid, int remainedGold, int remainedWood, int remainedFood, StructureTable? structureData)> CreateStructureAsync(
            string id,
            int tableId,
            float posX,
            float posZ,
            float rotY);
        Task<ERROR_CODE> DestroyStructureAsync(string id, long entityUid);

        Task<ERROR_CODE> ChangeSkill(string id, int[] skillSet, int[] spellSet);

        Task<(ERROR_CODE errCode, string generatedSessionId)> StartBattle(string id, string opponentPlayerId, S_BattleModeType modeType, long targetBattleLogUid = 0);
        Task<(ERROR_CODE errCode, PlayerInfo? playerInfo, BattleLogInfo resultLog, long rewardGold, long totalGold, long rewardWood, long totalWood, long rewardFood, long totalFood, int addedBounty, int totalBounty)>
            FinishBattleAsync(string id, string battleSessionId, string opponentId, S_BattleResult battleResult, long[] destroyedEntityUids, float playTime);

        Task<(ERROR_CODE errCode, PlayerInfo? playerInfo, long[] repairedEntityUids)>
            RepairEntitiesAsync(string id, long[] targetEntityUids, long expectedGold, long expectedWood, long expectedFood);

        Task<(ERROR_CODE errCode, BattleLogInfo resultLog)> AddBattleLog(
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
            S_BattleModeType modeType,
            bool dbSave = true);
    }
}
