using GameDB;
using JNetwork;
using LearningServer01.Data;

namespace LearningServer01
{
    public static class EntityMapper
    {
        public static EntityNetData ToNetData(this EntityItemInfo entityInfo, ITableService tableService, List<UserItem> items)
        {
            var entityTableData = tableService.Container.EntityTable_data.GetValueOrDefault((uint)entityInfo.TableID);
            if (entityTableData == null)
                return null;

            tableService.Container.StructureTable_data.TryGetValue(entityTableData.DetailTableID, out var structureData);

            return new EntityNetData()
            {
                OwnerID = entityInfo.OwnerID,
                UID = entityInfo.UID,
                TableID = entityInfo.TableID,
                Level = entityInfo.Level,
                NeedsRepair = entityInfo.NeedsRepair,
                PositionX = entityInfo.PositionX,
                PositionZ = entityInfo.PositionZ,
                RotationY = entityInfo.RotationY,
                // 내부 헬퍼 메서드 호출
                StructureData = structureData != null ? ToStructureSpecificData(entityInfo.Garrisons, items, structureData, tableService) : null
            };
        }

        public static List<EntityNetData> ToNetDataList(this IEnumerable<EntityItemInfo> entities, ITableService tableService, List<UserItem> items)
        {
            return entities.Select(t =>
                t.ToNetData(tableService, items))
                .Where(t => t != null)
                .ToList();
        }

        //------------- UTIL ---------------//

        public static StructureSpecificData ToStructureSpecificData(StructureTable tableData, ITableService tableService)
        {
            return ToStructureSpecificData(null, null, tableData, tableService);
        }

        public static StructureSpecificData ToStructureSpecificData(List<EntityGarrisonInfo> garrisons, List<UserItem> items, StructureTable tableData, ITableService tableService)
        {
            if (tableData.GarrisonCount == 0 && tableData.StructureType != E_StructureType.Spawner)
                return null;

            var data = new StructureSpecificData();

            // 테이블상 Garrison 주둔이 가능한다면
            // 일단 리스트는 기본으로 할당해줌 
            int garrisonCount = (int)tableData.GarrisonCount;
            if (garrisonCount > 0)
            {
                data.GarrisonedItemUIDs = Enumerable.Repeat(0L, garrisonCount).ToList();
                data.GarrisonedEntityTableIDs = Enumerable.Repeat(0, garrisonCount).ToList();
            }

            // Garrison 데이터가 존재하는 경우 처리
            if (garrisons != null)
            {
                for (int i = 0; i < garrisons.Count; i++)
                {
                    var g = garrisons[i];
                    if (g.Type == S_GarrisonSlotType.None)
                    {
                        Console.WriteLine($"[ERROR] Garrison 타입이 None 이 될 수 없음");
                        continue;
                    }

                    var targetGarrisonedItem = items.Find(t => t.UID == g.EquippedItemUID);
                    EntityTable entityData = null;
                    ItemTable itemData = null;

                    bool isValid =
                        g != null &&
                        g.UID != 0 &&
                        g.EquippedItemUID != 0 &&
                        g.OwnerEntityUID != 0 &&
                        targetGarrisonedItem != null &&
                        tableService.Container.ItemTable_data.TryGetValue((uint)targetGarrisonedItem.TableID, out itemData) &&
                        itemData.Type == E_ItemType.Squad &&
                        tableService.Container.EntityTable_data.TryGetValue(itemData.RefID, out entityData) &&
                        entityData.EntityType == E_EntityType.Character &&
                        tableService.Container.CharacterTable_data.ContainsKey(entityData.DetailTableID);

                    if (isValid == false)
                    {
                        // 만약 targetGarrisonedItem 이 Null 이라면 Garrison 주둔군 데이터는 존재하나
                        // 실제로 이 주둔군 Item 의 UID 가 ItemList에 없는 케이스 - 데이터 싱크 깨짐 의심
                        Console.WriteLine($"[ERROR] SpecificData 조립1 검증 데이터 오류 | IsNull : {g == null} , UID : {g.UID} , EquippedItemUID : {g.EquippedItemUID} , OwnerEntityUID : {g.OwnerEntityUID} , GarrisonedItemIsNull : {targetGarrisonedItem == null}");
                        continue;
                    }

                    if (g.Type == S_GarrisonSlotType.Garrison)
                    {
                        if (g.SlotIdx < 0 || g.SlotIdx >= garrisonCount)
                        {
                            Console.WriteLine($"[ERROR] SpecificData 조립2 검증 데이터 오류 | SlotIdx 의 인덱스 범위 벗어남 | Idx : {g.SlotIdx} , MaxCount : {garrisonCount}");
                            continue;
                        }

                        if (data.GarrisonedItemUIDs[g.SlotIdx] != 0)
                        {
                            // 루프중 설정이 된게 또 설정 시도하려고 하고있음. 인풋 데이터에 중복 체크
                            Console.WriteLine($"[ERROR] SpecificData 조립3 검증 데이터 오류 | 인풋 주둔 데이터의 SlotIdx 중복 에러");
                            continue;
                        }

                        data.GarrisonedItemUIDs[g.SlotIdx] = g.EquippedItemUID;
                        data.GarrisonedEntityTableIDs[g.SlotIdx] = (int)entityData.ID;
                    }
                    else if (g.Type == S_GarrisonSlotType.Spawn)
                    {
                        if (data.SpawningItemUID != 0)
                        {
                            // 루프중 설정이 된게 또 설정 시도하려고 하고있음. 인풋 데이터에 중복 체크
                            Console.WriteLine($"[ERROR] SpecificData 조립4 검증 데이터 오류 | 인풋 주둔 데이터의 SlotIdx 중복 에러");
                            continue;
                        }

                        data.SpawningItemUID = g.EquippedItemUID;
                        data.SpawningItemTableID = (int)itemData.ID;
                    }
                }
            }

            return data;
        }
    }
}
