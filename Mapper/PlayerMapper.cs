using GameDB;
using JNetwork;
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
            res.EnemyPlayerNickname = opponent.Nickname;
            res.StatusMsg = opponent.StatusMsg;
            res.MapName = opponent.MapName;
            res.StrengthStat = opponent.StrengthStat;
            res.OpponentLevel = opponent.Level;
            res.Bounty = opponent.Bounty;
            res.RemainedGold = me.Gold;
            res.Entities = opponent.PlacedEntities.ToNetDataList(tableService, opponent.InventoryItems);

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
    }
}
