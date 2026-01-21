namespace LearningServer01.Repositories
{
    public interface IPlayerRepository
    {
        Task<PlayerInfo> GetPlayerAsync(string id);

        Task<bool> AddPlayerAsync(PlayerInfo info);

        Task<bool> AddGold(string userId, int amount);
        Task<bool> AddWood(string userId, int amount);
        Task<bool> AddFood(string userId, int amount);

        Task<bool> ChangeSkill(string userId, int[] newSkillSet, int[] newSpellSet);

        Task<(long uid, int remainedGold, int remainedWood, int remainedFood)> CreateStructure(string userId, int tableId, float positionX, float positionZ, float rotationY);
        Task<bool> DestroyStructure(string userId, long uid);
    }
}
