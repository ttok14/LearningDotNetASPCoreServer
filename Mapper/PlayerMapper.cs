using GameDB;
using JNetwork;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Serilog;
using System.Numerics;

namespace LearningServer01
{
    public static class PlayerMapper
    {
        public static Res_Login ToLoginResponse(this PlayerInfo info, string token, ITableService tableService)
        {
            var res = new Res_Login();

            res.Result = ERROR_CODE.SUCCESS;
            res.OwnerId = info.ID;
            res.Token = token;
            res.Nickname = info.Nickname;
            res.StatusMsg = info.StatusMsg;
            res.Bounty = info.Bounty;
            res.EquippedHeroItemUID = info.EquippedHeroItemUID.HasValue ? info.EquippedHeroItemUID.Value : 0;
            res.StrengthStat = info.StrengthStat;
            res.Level = info.Level;
            res.Gold = info.Gold;
            res.Wood = info.Wood;
            res.Food = info.Food;
            res.SkillIDs = new[] { info.SkillID01, info.SkillID02, info.SkillID03 };
            res.SpellIDs = new[] { info.SpellID01, info.SpellID02, info.SpellID03 };
            res.Items = info.InventoryItems.Select(t => new UserItemNetData()
            {
                UID = t.UID,
                TableID = t.TableID,
                Quantity = t.Quantity
            }).ToList();
            res.Entities = info.PlacedEntities.ToNetDataList(tableService, info.InventoryItems);

            res.DeploymentSlots = info.DeploymentSlots.Select(t => new DeploymentSlotNetData()
            {
                SlotIdx = t.SlotIdx,
                EquippedItemUID = t.EquippedItemUID ?? 0
            }).ToList();

            res.BattleLogs = info.BattleLogs
                // 현 플레이어가 공격을 받은 케이스만 추리기 위함
                //  => 다줌 . 클라에서 필터링 위함
                // .Where(b => b.DefenderId == info.ID)
                .Select(b => b.ToBattleLogNetData()).ToList();

            return res;
        }

        public static Res_FinishBattle ToFinishBattleResponse(
            this (ERROR_CODE errCode, PlayerInfo? playerInfo, BattleLogInfo resultLog, long rewardGold, long totalGold, long rewardWood, long totalWood, long rewardFood, long totalFood, int addedBounty, int totalBounty) result)
        {
            var res = new Res_FinishBattle();

            res.Result = ERROR_CODE.SUCCESS;
            res.AddedBattleLog = result.resultLog.ToBattleLogNetData();
            res.RewardGold = result.rewardGold;
            res.TotalGold = result.totalGold;
            res.RewardWood = result.rewardWood;
            res.TotalWood = result.totalWood;
            res.RewardFood = result.rewardFood;
            res.TotalFood = result.totalFood;
            res.AddedBounty = result.addedBounty;
            res.TotalBounty = result.totalBounty;

            return res;
        }

        public static Res_SearchOpponent ToSearchOpponentResponse(this PlayerInfo me, PlayerInfo opponent, ITableService tableService)
        {
            var res = new Res_SearchOpponent();

            // 내 히어로 슬롯 조립 (EquippedHeroItemUID → UserItem에서 TableID 조회)
            var heroItem = me.InventoryItems.FirstOrDefault(t => t.UID == me.EquippedHeroItemUID);
            res.HeroSlot = new BattleDeploymentSlotNetData()
            {
                SlotIdx = -1,
                EquippedItemUID = me.EquippedHeroItemUID.Value,
                TableID = heroItem?.TableID ?? 0
            };

            // 내 출전 슬롯 조립 (DeploymentSlot + UserItem.TableID)
            res.DeploymentSlots = me.DeploymentSlots.OrderBy(t => t.SlotIdx).
                Select(slot =>
                {
                    int tableId = 0;
                    if (slot.EquippedItemUID.HasValue && slot.EquippedItemUID.Value != 0)
                    {
                        var item = me.InventoryItems.FirstOrDefault(i => i.UID == slot.EquippedItemUID.Value);
                        tableId = item?.TableID ?? 0;
                    }
                    return new BattleDeploymentSlotNetData()
                    {
                        SlotIdx = slot.SlotIdx,
                        EquippedItemUID = slot.EquippedItemUID ?? 0,
                        TableID = tableId
                    };
                }).ToList();

            // 상대 정보 조립
            res.Result = ERROR_CODE.SUCCESS;
            res.ID = opponent.ID;
            res.EnemyPlayerNickname = opponent.Nickname;
            res.StatusMsg = opponent.StatusMsg;
            res.MapName = opponent.MapName;
            res.StrengthStat = opponent.StrengthStat;
            res.OpponentLevel = opponent.Level;
            res.Bounty = opponent.Bounty;
            res.RemainedGold = me.Gold;
            res.Entities = opponent.PlacedEntities.ToNetDataList(tableService, opponent.InventoryItems);

            // 매칭시에는 Entity 의 상태 여부와 상관없이 무조건
            // 멀쩡한 상태로 클라에게 건네주어야함 
            for (int i = 0; i < res.Entities.Count; i++)
                res.Entities[i].NeedsRepair = false;

            return res;
        }

        public static Res_LoadRevenge ToLoadRevengeResponse(this PlayerInfo me, PlayerInfo opponent, ITableService tableService)
        {
            var res = new Res_LoadRevenge();

            // 내 히어로 슬롯 조립
            var heroItem = me.InventoryItems.FirstOrDefault(t => t.UID == me.EquippedHeroItemUID);
            res.HeroSlot = new BattleDeploymentSlotNetData()
            {
                SlotIdx = -1,
                EquippedItemUID = me.EquippedHeroItemUID.Value,
                TableID = heroItem?.TableID ?? 0
            };

            // 내 출전 슬롯 조립
            res.DeploymentSlots = me.DeploymentSlots.OrderBy(t => t.SlotIdx).
                Select(slot =>
                {
                    int tableId = 0;

                    if (slot.EquippedItemUID.HasValue && slot.EquippedItemUID.Value != 0)
                    {
                        var item = me.InventoryItems.FirstOrDefault(i => i.UID == slot.EquippedItemUID.Value);
                        tableId = item?.TableID ?? 0;
                    }
                    return new BattleDeploymentSlotNetData()
                    {
                        SlotIdx = slot.SlotIdx,
                        EquippedItemUID = slot.EquippedItemUID ?? 0,
                        TableID = tableId
                    };
                }).ToList();

            res.Result = ERROR_CODE.SUCCESS;
            res.ID = opponent.ID;
            res.EnemyPlayerNickname = opponent.Nickname;
            res.StatusMsg = opponent.StatusMsg;
            res.MapName = opponent.MapName;
            res.StrengthStat = opponent.StrengthStat;
            res.OpponentLevel = opponent.Level;
            res.Bounty = opponent.Bounty;
            res.Entities = opponent.PlacedEntities.ToNetDataList(tableService, opponent.InventoryItems);

            // 복수 시에도 건물은 멀쩡한 상태로 전달
            for (int i = 0; i < res.Entities.Count; i++)
                res.Entities[i].NeedsRepair = false;

            return res;
        }

        public static Res_EnterHome ToEnterHomeResponse(this PlayerInfo info, ITableService tableService)
        {
            var res = new Res_EnterHome();

            res.Result = ERROR_CODE.SUCCESS;
            res.MapName = info.MapName;
            res.Entities = info.PlacedEntities.ToNetDataList(tableService, info.InventoryItems);

            return res;
        }

        public static Res_CreateStructure ToCreateStructureResponse(
            this (ERROR_CODE errCode, long uid, int remainedGold, int remainedWood, int remainedFood, StructureTable? structureData) result,
            ITableService tableService)
        {
            var res = new Res_CreateStructure();
            res.Result = result.errCode;

            if (result.errCode == ERROR_CODE.SUCCESS)
            {
                res.UID = result.uid;
                res.RemainedGold = result.remainedGold;
                res.RemainedWood = result.remainedWood;
                res.RemainedFood = result.remainedFood;

                if (result.structureData != null)
                {
                    res.StructureData = EntityMapper.ToStructureSpecificData(result.structureData, tableService);
                }
            }

            return res;
        }

        public static Res_RepairEntities ToRepairEntitiesResponse(
          this (ERROR_CODE errCode, PlayerInfo? playerInfo, long[] repairedEntityUids) result,
          ITableService tableService)
        {
            if (result.errCode != ERROR_CODE.SUCCESS)
                return new() { Result = result.errCode };

            var res = new Res_RepairEntities();

            res.Result = ERROR_CODE.SUCCESS;
            res.RemainGold = result.playerInfo.Gold;
            res.RemainWood = result.playerInfo.Wood;
            res.RemainFood = result.playerInfo.Food;
            res.RepairedEntityUIDs = result.repairedEntityUids;

            return res;
        }
    }
}
