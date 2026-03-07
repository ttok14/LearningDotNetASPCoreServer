using GameDB;
using JNetwork;
using LearningServer01.Data;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Data.Common;

namespace LearningServer01.Repositories
{
    public class DbPlayerRepository : IPlayerRepository
    {
        private readonly AppDbContext _context;
        private readonly ILockService _lockService;

        public DbPlayerRepository(AppDbContext dbContext, ILockService lockService)
        {
            _context = dbContext;
            _lockService = lockService;
        }

        public async Task<DbTransaction> BeginTransactionAsync()
        {
            var efTransaction = await _context.Database.BeginTransactionAsync();
            return efTransaction.GetDbTransaction();
        }

        void IPlayerRepository.RemoveEntity(EntityItemInfo entity)
        {
            _context.Entities.Remove(entity);
        }

        void IPlayerRepository.AddPlayer(PlayerInfo info)
        {
            _context.Players.Add(info);
        }

        Task<bool> IPlayerRepository.IsPlayerExistByIDAsync(string id)
        {
            return _context.Players.AnyAsync(p => p.ID == id);
        }

        Task<bool> IPlayerRepository.IsPlayerExistByNickname(string nickname)
        {
            return _context.Players.AnyAsync(p => p.Nickname == nickname);
        }

        public async Task<PlayerInfo> GetPlayerBasicAsync(string id, bool isReadonly = false)
        {
            var query = _context.Players.AsQueryable<PlayerInfo>();

            if (isReadonly)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(p => p.ID == id);
        }

        public async Task<PlayerInfo> GetPlayerFullAsync(string id, bool isReadonly = false)
        {
            var query = _context.Players.AsQueryable<PlayerInfo>();

            if (isReadonly)
            {
                query = query.AsNoTracking();
            }

            return await
                query.Include(p => p.PlacedEntities)
                    .ThenInclude(e => e.Garrisons)
                .Include(p => p.InventoryItems)
                .Include(p => p.DeploymentSlots)
                .FirstOrDefaultAsync(p => p.ID == id);
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

        public async Task<(ERROR_CODE errCode, long uid, int remainedGold, int remainedWood, int remainedFood)> CreateStructure(
            string userId, int tableId, float positionX, float positionZ, float rotationY,
            E_CurrencyType costType, int costPrice)
        {
            if (tableId == 0)
                return (ERROR_CODE.CREATE_STRUCTURE_FAIL_01, 0, 0, 0, 0);

            await using var @lock = await _lockService.LockPlayerAsync(userId);

            var player = await _context.Players.FirstOrDefaultAsync(p => p.ID == userId);
            if (player == null)
                return (ERROR_CODE.CREATE_STRUCTURE_FAIL_02, 0, 0, 0, 0);

            // 재화 부족 체크
            int currentAmount = costType switch
            {
                E_CurrencyType.Gold => player.Gold,
                E_CurrencyType.Wood => player.Wood,
                E_CurrencyType.Food => player.Food,
                _ => 0
            };

            if (currentAmount < costPrice)
                return (ERROR_CODE.NOT_ENOUGH_CURRENCY, 0, 0, 0, 0);

            // 재화 차감
            switch (costType)
            {
                case E_CurrencyType.Gold:
                    player.Gold -= costPrice;
                    break;
                case E_CurrencyType.Wood:
                    player.Wood -= costPrice;
                    break;
                case E_CurrencyType.Food:
                    player.Food -= costPrice;
                    break;
            }

            var newStructure = new EntityItemInfo()
            {
                OwnerID = userId,
                Level = 1,
                PositionX = positionX,
                PositionZ = positionZ,
                RotationY = rotationY,
                TableID = tableId
            };

            await _context.Entities.AddAsync(newStructure);

            bool isSuccess = await _context.SaveChangesAsync() > 0;

            return (isSuccess ? ERROR_CODE.SUCCESS : ERROR_CODE.FAIL_DATABASE_SAVE, newStructure.UID, player.Gold, player.Wood, player.Food);
        }

        public async Task<bool> DestroyStructure(string userId, long uid)
        {
            if (uid == 0)
                return false;

            bool exist = await _context.Players.AnyAsync(p => p.ID == userId);
            if (exist == false)
                return false;

            return await _context.Entities
                .Where(s => s.OwnerID == userId && s.UID == uid)
                .ExecuteDeleteAsync() > 0;
        }
        public async Task<PlayerInfo> GetRandomOpponentAsync(string excludeUserId)
        {
            var count = await _context.Players.CountAsync(p => p.ID != excludeUserId);

            if (count == 0)
                return null;

            var randomIndex = new Random().Next(count);

            return await _context.Players
                .Where(p => p.ID != excludeUserId)
                .Skip(randomIndex)
                .Include(p => p.PlacedEntities)
                    .ThenInclude(e => e.Garrisons)
                .Include(p => p.InventoryItems)
                .Include(p => p.DeploymentSlots)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> SaveChangesAsync(
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string filePath = "",
           [CallerLineNumber] int lineNumber = 0)
        {
            bool res = await _context.SaveChangesAsync() > 0;

            if (!res)
            {
                Console.WriteLine($"[WARN] DB 저장 실패 | 위치: {memberName} ({filePath}:{lineNumber})");
            }
            return res;
        }
    }
}
