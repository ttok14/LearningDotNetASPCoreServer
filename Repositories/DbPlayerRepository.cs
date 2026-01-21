using JNetwork;
using LearningServer01.Data;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LearningServer01.Repositories
{
    public class DbPlayerRepository : IPlayerRepository
    {
        private readonly AppDbContext _context;

        public DbPlayerRepository(AppDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<PlayerInfo> GetPlayerAsync(string id)
        {
            return await _context.Players
                .Include(p => p.Structures)
                .FirstOrDefaultAsync(p => p.ID == id);
        }

        public async Task<bool> AddPlayerAsync(PlayerInfo info)
        {
            bool exists = await _context.Players.AnyAsync(p => p.ID == info.ID);

            if (exists)
                return false;

            _context.Players.Add(info);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AddGold(string userId, int amount)
        {
            return await _context.Players
                .Where(p => p.ID == userId)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(p => p.Gold, p => p.Gold + amount)) > 0;
        }

        public async Task<bool> AddWood(string userId, int amount)
        {
            return await _context.Players
                .Where(p => p.ID == userId)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(p => p.Wood, p => p.Wood + amount)) > 0;
        }

        public async Task<bool> AddFood(string userId, int amount)
        {
            return await _context.Players
                .Where(p => p.ID == userId)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(p => p.Food, p => p.Food + amount)) > 0;
        }

        public async Task<bool> ChangeSkill(
            string userId,
            int[] newSkillSet,
            int[] newSpellSet)
        {
            var player = await _context.Players.FirstOrDefaultAsync(p => p.ID == userId);

            if (player == null)
            {
                return false;
            }

            player.SkillID01 = newSkillSet[0];
            player.SkillID02 = newSkillSet[1];
            player.SkillID03 = newSkillSet[2];

            player.SpellID01 = newSpellSet[0];
            player.SpellID02 = newSpellSet[1];
            player.SpellID03 = newSpellSet[2];

            await _context.SaveChangesAsync();

            return true;
        }

        //public async Task<bool> UpdateStructuresAsync(string userId, List<StructureItem> structureInfo)
        //{
        //    var user = await _context.Players
        //        .Include(p => p.Structures)
        //        .FirstOrDefaultAsync(p => p.ID == userId);

        //    if (user == null)
        //        return false;

        //    _context.Structures.RemoveRange(user.Structures);

        //    var newStructures = structureInfo.Select(s => new StructureInfo()
        //    {
        //        UID = s.StructureID,
        //        OwnerID = userId,
        //        Level = s.Level,
        //        TableID = s.TableID,
        //        PositionX = s.PositionX,
        //        PositionZ = s.PositionZ,
        //        RotationY = s.RotationY,
        //    }).ToList();

        //    await _context.Structures.AddRangeAsync(newStructures);

        //    await _context.SaveChangesAsync();

        //    return true;
        //}

        public async Task<(long uid, int remainedGold, int remainedWood, int remainedFood)> CreateStructure(string userId, int tableId, float positionX, float positionZ, float rotationY)
        {
            if (tableId == 0)
                return (-1, 0, 0, 0);

            var player = await _context.Players.FirstOrDefaultAsync(p => p.ID == userId);
            if (player == null)
                return (-2, 0, 0, 0);

            #region ===:: 임시 재화차감 코드 (하드코딩, 서버 테이블 연동되면 삭제하고 로직 분류) ::====
            var price = Temp_StructureCost.CostContainer[tableId].Price;

            switch (Temp_StructureCost.CostContainer[tableId].Type)
            {
                case Temp_StructureCost.CurrencyType.Gold:
                    player.Gold = Math.Max(player.Gold - price, 0);
                    break;
                case Temp_StructureCost.CurrencyType.Wood:
                    player.Wood = Math.Max(player.Wood - price, 0);
                    break;
                case Temp_StructureCost.CurrencyType.Food:
                    player.Food = Math.Max(player.Food - price, 0);
                    break;
            }
            #endregion

            var newStructure = new StructureInfo()
            {
                OwnerID = userId,
                Level = 1,
                PositionX = positionX,
                PositionZ = positionZ,
                RotationY = rotationY,
                TableID = tableId
            };

            await _context.Structures.AddAsync(newStructure);

            /// 여기서 <see cref="StructureInfo.UID"/> 가 자동으로 내부적으로
            /// 들어간다함. (EF+DB 협업)
            await _context.SaveChangesAsync();

            return (newStructure.UID, player.Gold, player.Wood, player.Food);
        }

        public async Task<bool> DestroyStructure(string userId, long uid)
        {
            if (uid == 0)
                return false;

            bool exist = await _context.Players.AnyAsync(p => p.ID == userId);
            if (exist == false)
                return false;

            return await _context.Structures
                .Where(s => s.OwnerID == userId && s.UID == uid)
                .ExecuteDeleteAsync() > 0;
        }
    }
}
