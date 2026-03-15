using GameDB;
using JNetwork;
using LearningServer01.Data;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Serilog;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        async Task<(bool res, string nickname)> IPlayerRepository.IsPlayerExistAndGetNicknameByIDAsync(string id)
        {
            var player = await GetPlayerAsync(id, E_PlayerInclude.None, isReadonly: true);
            if (player == null)
                return (false, string.Empty);

            return (true, player.Nickname);
        }

        Task<bool> IPlayerRepository.IsPlayerExistByNickname(string nickname)
        {
            return _context.Players.AnyAsync(p => p.Nickname == nickname);
        }

        //async Task<PlayerInfo> GetPlayerInfoQueryableAsync(IQueryable<PlayerInfo> info, E_PlayerInclude includes = E_PlayerInclude.None, bool isReadonly = false)
        //{
        //    if (isReadonly)
        //    {
        //        info = info.AsNoTracking();
        //    }

        //    if (includes.HasFlag(E_PlayerInclude.PlacedEntities))
        //        info = info.Include(p => p.PlacedEntities).ThenInclude(e => e.Garrisons);

        //    if (includes.HasFlag(E_PlayerInclude.InventoryItems))
        //        info = info.Include(p => p.InventoryItems);

        //    if (includes.HasFlag(E_PlayerInclude.DeploymentSlots))
        //        info = info.Include(p => p.DeploymentSlots);

        //    try
        //    {
        //        var player = await info.FirstOrDefaultAsync(p => p.ID == id);

        //        if (player != null && includes.HasFlag(E_PlayerInclude.BattleLogs))
        //        {
        //            player.BattleLogs = await _context.BattleLogs
        //                .Where(b => b.DefenderId == id || b.AttackerId == id)
        //                .ToListAsync();
        //        }

        //        return player;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error($"GetPlayerAsync() 예외 발생 | ID : {id} , includes: {includes}, isReadonly : {isReadonly} | ex : {ex}");
        //        return null;
        //    }
        //}

        public async Task<PlayerInfo> GetPlayerAsync(string id, E_PlayerInclude includes = E_PlayerInclude.None, bool isReadonly = false)
        {
            var query = _context.Players.AsQueryable<PlayerInfo>();

            if (isReadonly)
            {
                query = query.AsNoTracking();
            }

            if (includes.HasFlag(E_PlayerInclude.PlacedEntities))
                query = query.Include(p => p.PlacedEntities).ThenInclude(e => e.Garrisons);

            if (includes.HasFlag(E_PlayerInclude.InventoryItems))
                query = query.Include(p => p.InventoryItems);

            if (includes.HasFlag(E_PlayerInclude.DeploymentSlots))
                query = query.Include(p => p.DeploymentSlots);

            try
            {
                var player = await query.FirstOrDefaultAsync(p => p.ID == id);

                if (player != null && includes.HasFlag(E_PlayerInclude.BattleLogs))
                {
                    player.BattleLogs = await _context.BattleLogs
                        .Where(b => b.DefenderId == id || b.AttackerId == id)
                        .ToListAsync();
                }

                return player;
            }
            catch (Exception ex)
            {
                Log.Error($"GetPlayerAsync() 예외 발생 | ID : {id} , includes: {includes}, isReadonly : {isReadonly} | ex : {ex}");
                return null;
            }
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
                NeedsRepair = false,
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
        public async Task<(PlayerInfo? opponent, bool foundInExcluede)> GetRandomOpponentAsync(string askerId, IReadOnlyList<string> excludeIds)
        {
            var totalExclude = excludeIds.ToList();
            totalExclude.Add(askerId);

            var count = await _context.Players.CountAsync(p => totalExclude.Contains(p.ID) == false);

            if (count > 0)
            {
                var randomIndex = new Random().Next(count);

                // TODO : 추후에 매칭 상대를 필터해야하는 부분이 추가로 생기면
                // 이쪽에 작업하면 될듯 (e.g 비슷한 Rate 대의 유저들 끼리의 매칭
                // 또는 '접속' 중인 유저는 제외 등)
                var opponent = await _context.Players
                    // 제외 리스트에서 없는 애만 대상으로함
                    .Where(p => totalExclude.Contains(p.ID) == false)
                    .Skip(randomIndex)
                    .Include(p => p.PlacedEntities)
                        .ThenInclude(e => e.Garrisons)
                     .Include(p => p.InventoryItems)
                    // .Include(p => p.DeploymentSlots)
                    .FirstOrDefaultAsync();

                if (opponent != null)
                    return (opponent, false);
            }

            if (excludeIds.Count > 0)
            {
                var oldestId = excludeIds[0];

                return (await _context.Players
                    // 제외 리스트에서 없는 애만 대상으로함
                    .Where(p => p.ID == oldestId)
                    .Include(p => p.PlacedEntities)
                        .ThenInclude(e => e.Garrisons)
                    .Include(p => p.InventoryItems)
                    //  .Include(p => p.DeploymentSlots)
                    .FirstOrDefaultAsync(), true);
            }

            return (null, false);
        }

        public async Task<bool> SaveChangesAsync(
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string filePath = "",
           [CallerLineNumber] int lineNumber = 0)
        {
            bool res = await _context.SaveChangesAsync() > 0;

            if (res == false)
            {
                Console.WriteLine($"[WARN] DB 저장 실패 | 위치: {memberName} ({filePath}:{lineNumber})");
            }
            return res;
        }

        public BattleLogInfo AddBattleLog(
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
            S_BattleModeType modeType)
        {
            var res = new BattleLogInfo()
            {
                SessionId = sessionId,
                AttackerId = attackerId,
                AttackerNickname = attackerNickname,
                DefenderId = defenderId,
                DefenderNickname = defenderNickname,
                LogTimeUtc = timeUtc,
                LootedGold = lootedGold,
                LootedWood = lootedWood,
                LootedFood = lootedFood,
                BattleResult = result,
                ModeType = modeType,
                IsRevenged = false
            };

            _context.BattleLogs.Add(res);

            return res;
        }

        public async Task<BattleLogInfo> GetBattleLogAsync(long logUid)
        {
            return await _context.BattleLogs.FirstOrDefaultAsync(l => l.ID == logUid);
        }
    }
}
