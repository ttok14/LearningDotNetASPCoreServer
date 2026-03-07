using JNetwork;
using GameDB;

namespace LearningServer01
{
    public interface IPlayerService
    {
        Task<(ERROR_CODE errCode, PlayerInfo newUser)> RegisterNewPlayerAsync(string id, string password);
        Task<(ERROR_CODE errCode, PlayerInfo opponent)> SearchOpponentAsync(PlayerInfo player);

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
    }
}
