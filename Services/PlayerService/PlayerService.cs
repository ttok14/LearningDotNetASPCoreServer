using GameDB;
using JNetwork;
using LearningServer01.Config;
using LearningServer01.Data;
using LearningServer01.MemoryCache;
using LearningServer01.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace LearningServer01.Services.PlayerService
{
    public class PlayerService : IPlayerService
    {
        IPlayerRepository _repo;
        ITableService _tableService;
        ILogger<PlayerService> _logger;
        //GameSettings _gameSettings;
        ActiveBattleCache _activeBattleCache;
        PlayerMatchingContextCache _matchingContextCache;

        public PlayerService(
            IPlayerRepository repo,
            ILogger<PlayerService> logger,
            ITableService tableService,
            ActiveBattleCache activeBattleCache,
            PlayerMatchingContextCache matchingContextCache)
        // IOptions<GameSettings> gameSettingsOptions
        {
            _repo = repo;
            _logger = logger;
            _tableService = tableService;
            _activeBattleCache = activeBattleCache;
            _matchingContextCache = matchingContextCache;
            //  _gameSettings = gameSettingsOptions.Value;
        }

        public async Task<ERROR_CODE> PostLoginAsync(PlayerInfo? loggedInPlayerInfo)
        {
            if (loggedInPlayerInfo == null)
                return ERROR_CODE.FAIL_INVALID_USER;

            return await CleanZombieBattleSessions(loggedInPlayerInfo.ID);
        }

        public async Task<ERROR_CODE> CleanZombieBattleSessions(string attackerId)
        {
            // 만약 처리되지 않은 공격 전투 로그가 존재하면 기록 처리
            if (_activeBattleCache.TryRemove(attackerId, out var battleSession))
            {
                try
                {
                    await AddBattleLog(
                        battleSession.SessionId,
                        battleSession.AttackerId,
                        battleSession.AttackerNickname,
                        battleSession.DefenderId,
                        battleSession.DefenderNickname,
                        DateTime.UtcNow,
                        S_BattleResult.Lose,
                        0, 0, 0, battleSession.ModeType);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"좀비 세션(SessionId: {battleSession.SessionId}) 정리 실패. 메시지 : {ex.Message}");
                }
            }

            return ERROR_CODE.SUCCESS;
        }

        public async Task<(ERROR_CODE errCode, PlayerInfo? newUser)> RegisterNewPlayerAsync(string id, string password)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
                return (ERROR_CODE.REGISTER_ID_OR_PW_EMPTY, null);

            bool isDuplicate = await _repo.IsPlayerExistByIDAsync(id);
            if (isDuplicate)
            {
                return (ERROR_CODE.REGISTER_FAIL_DUPLICATE, null);
            }

            var heroTableEntry = _tableService.Container.ItemTable_data.Values.FirstOrDefault(v => v.Type == E_ItemType.Hero);

            if (heroTableEntry == null)
            {
                Console.WriteLine($"[ERROR] 아이템 테이블에 히어로(Type==Hero) 가 없어 회원가입 실패");
                return (ERROR_CODE.NOT_EXIST_IN_TABLE, null);
            }

            uint heroTableId = heroTableEntry.ID;
            var heroItem = new UserItem()
            {
                OwnerID = id,
                Level = 1,
                Quantity = 1,
                TableID = (int)heroTableId
            };

            var defaultEntities = DefaultEntities(id);
            var defaultItems = DefaultItems(id, heroItem);
            var defaultDeploymentSlots = DefaultDeploymentSlots(id, defaultItems);

            var newPlayer = new PlayerInfo()
            {
                ID = id,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                MapName = GetRandomMap(),
                Level = 1,
                Nickname = string.Empty,
                StatusMsg = "제발 쳐들어오지 마세요 ㅜㅜ",
                Bounty = 100,
                StrengthStat = 959595,
                Gold = 50000,
                Wood = 50000,
                Food = 50000,
                SkillID01 = 1,
                SkillID02 = 15,
                SkillID03 = 12,
                SpellID01 = 22,
                SpellID02 = 23,
                SpellID03 = 24,
                PlacedEntities = defaultEntities,
                InventoryItems = defaultItems,
                DeploymentSlots = defaultDeploymentSlots
            };

            using (var transaction = await _repo.BeginTransactionAsync())
            {
                try
                {
                    _repo.AddPlayer(newPlayer);

                    await _repo.SaveChangesAsync();

                    // 사실상 assert 에 가까움. EF 가 무조건 UID 를 0 이아닌 값으로 채웟어야함
                    // DB 에서 정상적으로 처리가됐다면
                    // 그래서 만약 여기에 걸릴거라면 애초에 Exception 나서 여기까지 오지않을 것 같기도하다만
                    // 일단 방어코드로 남김
                    if (heroItem.UID == 0)
                    {
                        Console.WriteLine($"[ERROR] 히어로 아이템의 UID(PK) 가 부여가 되지않음 - 회원가입 실패");
                        await transaction.RollbackAsync();
                        return (ERROR_CODE.REGISTER_USER_HERO_ISSUE, null);
                    }

                    // 이 시점엔 DB 에 의해 PK 발급 완료된 상태이니
                    // 바로 넣어주면 됨 
                    newPlayer.EquippedHeroItemUID = heroItem.UID;

                    await _repo.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return (ERROR_CODE.SUCCESS, newPlayer);
                }
                catch (Exception exp)
                {
                    Console.WriteLine($"[ERROR] {exp.ToString()}");

                    await transaction.RollbackAsync();

                    return (ERROR_CODE.EXCEPTION, null);
                }
            }
        }

#if DEBUG
        async Task<(ERROR_CODE errCode, int remainedCurrency)> IPlayerService.CheatCurrency(string id, E_CurrencyType currencyType)
        {
            var player = await _repo.GetPlayerAsync(id, E_PlayerInclude.None);
            if (player == null)
                return (ERROR_CODE.FAIL_INVALID_USER, 0);

            int current = 0;

            switch (currencyType)
            {
                case E_CurrencyType.Gold:
                    player.Gold += 1000;
                    current = player.Gold;
                    break;
                case E_CurrencyType.Wood:
                    player.Wood += 1000;
                    current = player.Wood;
                    break;
                case E_CurrencyType.Food:
                    player.Food += 1000;
                    current = player.Food;
                    break;
                default:
                    return (ERROR_CODE.INVALID_CURRENCY_TYPE, 0);
            }

            var res = await _repo.SaveChangesAsync();

            if (res == false)
                return (ERROR_CODE.FAIL_DATABASE_SAVE, 0);

            return (ERROR_CODE.SUCCESS, current);
        }
#endif

        public async Task<ERROR_CODE> EquipDeploymentSlotAsync(string id, int equipSlotIdx, long equipItemUid)
        {
            var player = await _repo.GetPlayerAsync(id, E_PlayerInclude.DeploymentSlots | E_PlayerInclude.InventoryItems);
            if (player == null)
                return ERROR_CODE.FAIL_INVALID_USER;

            if (equipItemUid == 0)
                return ERROR_CODE.INVALID_ITEM;

            if (equipSlotIdx < 0 || equipSlotIdx >= player.DeploymentSlots.Count)
                return ERROR_CODE.SLOT_OUT_OF_RANGE;

            var equippingItem = player.InventoryItems.Find(t => t.UID == equipItemUid);
            if (equippingItem == null)
                return ERROR_CODE.ITEM_NO_EXIST;

            var targetSlot = player.DeploymentSlots.Find(t => t.SlotIdx == equipSlotIdx);

            // 이미 해당 칸에 끼워져 있음 => 즉시 성공 판정/종료
            if (targetSlot.EquippedItemUID == equipItemUid)
                return ERROR_CODE.SUCCESS;

            var existingSlot = player.DeploymentSlots.Find(t => t.EquippedItemUID == equipItemUid);

            // 장착하려는 아이템이 이미 장착 돼있는 케이스 
            if (existingSlot != null)
                existingSlot.UnequipItem();

            targetSlot.EquipItem(equippingItem);

            return await _repo.SaveChangesAsync() ? ERROR_CODE.SUCCESS : ERROR_CODE.FAIL_DATABASE_SAVE;
        }

        public async Task<ERROR_CODE> UnequipDeploymentSlotAsync(string id, int equipSlotIdx)
        {
            var player = await _repo.GetPlayerAsync(id, E_PlayerInclude.DeploymentSlots);
            if (player == null)
                return ERROR_CODE.FAIL_INVALID_USER;

            if (equipSlotIdx < 0 || equipSlotIdx >= player.DeploymentSlots.Count)
                return ERROR_CODE.SLOT_OUT_OF_RANGE;

            var slot = player.DeploymentSlots.Find(t => t.SlotIdx == equipSlotIdx);

            if (slot == null)
                return ERROR_CODE.SLOT_NOT_EXIST;

            if (slot.EquippedItemUID.HasValue == false)
                return ERROR_CODE.NO_ITEM_TO_UNEQUIP;

            slot.UnequipItem();

            return await _repo.SaveChangesAsync() ? ERROR_CODE.SUCCESS : ERROR_CODE.FAIL_DATABASE_SAVE;
        }

        public async Task<ERROR_CODE> GarrisonUnitAsync(string id, long ownerEntityUid, int slotIdx, long garrisonUnitUid)
        {
            if (ownerEntityUid == 0 || garrisonUnitUid == 0 || slotIdx < 0)
                return ERROR_CODE.INVALID_INPUT;

            var player = await _repo.GetPlayerAsync(id, E_PlayerInclude.PlacedEntities | E_PlayerInclude.InventoryItems);
            if (player == null)
                return ERROR_CODE.FAIL_INVALID_USER;

            var targetOwnerEntity = player.PlacedEntities.Find(t => t.UID == ownerEntityUid);

            // 주둔 오너 엔티티 존재 여부 체크
            if (targetOwnerEntity == null)
                return ERROR_CODE.NO_TARGET_ENTITY_FOUND;

            #region ====:: 주둔 유닛 검증 ::====
            // 인벤토리에 주둔할 유닛이 존재하는지 체크
            var targetGarrisonItem = player.InventoryItems.Find(t => t.UID == garrisonUnitUid);
            if (targetGarrisonItem == null)
                return ERROR_CODE.NO_GARRISON_UNIT_FOUND;

            // 아이템 테이블 데이터에 접근 
            if (_tableService.Container.ItemTable_data.TryGetValue((uint)targetGarrisonItem.TableID, out var itemData) == false)
                return ERROR_CODE.FAIL_TABLE_DATA_NO_EXIST;

            // 주둔은 Squad 타입의 아이템만 가능, 체크 
            if (itemData.Type != E_ItemType.Squad)
                return ERROR_CODE.ITEM_NOT_SQUAD_TYPE;

            // 엔티티 테이블 데이터 존재 체크 
            if (_tableService.Container.EntityTable_data.TryGetValue(itemData.RefID, out var entityTableData) == false)
                return ERROR_CODE.NOT_EXIST_IN_TABLE;

            // CharacterType 인지 체크
            if (entityTableData.EntityType != E_EntityType.Character)
                return ERROR_CODE.NOT_CHARACTER_TYPE;
            #endregion

            // 주둔 건물 Entity 데이터 접근 
            if (_tableService.Container.EntityTable_data.TryGetValue((uint)targetOwnerEntity.TableID, out var ownerEntityData) == false)
                return ERROR_CODE.NOT_EXIST_IN_TABLE;

            if (ownerEntityData.EntityType != E_EntityType.Structure)
                return ERROR_CODE.NOT_STRUCTURE_TYPE;

            // 주둔 건물의 Structure 데이터 접근 
            if (_tableService.Container.StructureTable_data.TryGetValue(ownerEntityData.DetailTableID, out var structureData) == false)
                return ERROR_CODE.FAIL_TABLE_DATA_NO_EXIST;

            uint garrisonCount = structureData.GarrisonCount;

            // 주둔 슬롯 인덱스가 범위를 벗어나는지 체크 
            if (slotIdx < 0 || slotIdx >= garrisonCount)
                return ERROR_CODE.GARRISON_SLOT_OUT_OF_INDEX;

            // 건물을 쭉 순회하며 만약 현재 장착하려는 주둔 부대가 이미 어딘가에
            // 장착돼 있는지를 검사 (Garrisoins 에는 주둔/스폰이 모두 존재하기 때문에 두 케이스 모두 체크 가능)
            var currentUnitGarrison = player.PlacedEntities
                .SelectMany(e => e.Garrisons)
                .FirstOrDefault(g => g.EquippedItemUID == garrisonUnitUid);

            // 만약에 동일한 OwnerEntity 인데 SlotIdx 까지 같다면 똑같은 곳에 재장착 시도
            //  => 바로 성공/종료 
            if (currentUnitGarrison != null)
            {
                if (currentUnitGarrison.OwnerEntityUID == ownerEntityUid &&
                    currentUnitGarrison.SlotIdx == slotIdx &&
                    currentUnitGarrison.Type == S_GarrisonSlotType.Garrison)
                    return ERROR_CODE.SUCCESS;

                var prevOwnerEntity = player.PlacedEntities.Find(t => t.UID == currentUnitGarrison.OwnerEntityUID);
                if (prevOwnerEntity != null)
                {
                    prevOwnerEntity.Garrisons.Remove(currentUnitGarrison);
                }
            }

            var targetSlot = targetOwnerEntity.Garrisons.Find(t => t.SlotIdx == slotIdx && t.Type == S_GarrisonSlotType.Garrison);

            if (targetSlot == null)
            {
                targetSlot = new EntityGarrisonInfo()
                {
                    SlotIdx = slotIdx
                };

                targetOwnerEntity.Garrisons.Add(targetSlot);
            }

            targetSlot.Set(ownerEntityUid, S_GarrisonSlotType.Garrison, garrisonUnitUid);

            return await _repo.SaveChangesAsync() ? ERROR_CODE.SUCCESS : ERROR_CODE.FAIL_DATABASE_SAVE;
        }

        public async Task<ERROR_CODE> SetSpawnUnitAsync(string id, long ownerEntityUid, long spawnUnitUid)
        {
            if (ownerEntityUid == 0 || spawnUnitUid == 0)
                return ERROR_CODE.INVALID_INPUT;

            var player = await _repo.GetPlayerAsync(id, E_PlayerInclude.PlacedEntities | E_PlayerInclude.InventoryItems);
            if (player == null) return ERROR_CODE.FAIL_INVALID_USER;

            // 엔티티 존재 체크
            var targetOwnerEntity = player.PlacedEntities.Find(t => t.UID == ownerEntityUid);
            if (targetOwnerEntity == null) return ERROR_CODE.NO_TARGET_ENTITY_FOUND;

            // 대상 엔티티가 스포너 건물인지 검증
            if (_tableService.Container.EntityTable_data.TryGetValue((uint)targetOwnerEntity.TableID, out var ownerEntityData) == false)
                return ERROR_CODE.NOT_EXIST_IN_TABLE;

            if (_tableService.Container.StructureTable_data.TryGetValue(ownerEntityData.DetailTableID, out var structureData) == false)
                return ERROR_CODE.FAIL_TABLE_DATA_NO_EXIST;

            // 스포너 타입이 아니면 컽
            if (structureData.StructureType != E_StructureType.Spawner)
                return ERROR_CODE.NOT_SPAWNER_TYPE;

            // 스폰할 유닛 체크
            var targetSpawnItem = player.InventoryItems.Find(t => t.UID == spawnUnitUid);
            if (targetSpawnItem == null) return ERROR_CODE.NO_GARRISON_UNIT_FOUND;

            if (_tableService.Container.ItemTable_data.TryGetValue((uint)targetSpawnItem.TableID, out var itemData) == false)
                return ERROR_CODE.FAIL_TABLE_DATA_NO_EXIST;

            // Squad 타입이고 실체가 Character인지 확인
            if (itemData.Type != E_ItemType.Squad) return ERROR_CODE.ITEM_NOT_SQUAD_TYPE;

            // 이사 처리 (다른 곳에 등록된 것 제거)
            var currentUnitGarrison = player.PlacedEntities
                .SelectMany(e => e.Garrisons)
                .FirstOrDefault(g => g.EquippedItemUID == spawnUnitUid);

            if (currentUnitGarrison != null)
            {
                if (currentUnitGarrison.OwnerEntityUID == ownerEntityUid && currentUnitGarrison.Type == S_GarrisonSlotType.Spawn)
                    return ERROR_CODE.SUCCESS;

                var prevOwner = player.PlacedEntities.Find(e => e.UID == currentUnitGarrison.OwnerEntityUID);
                prevOwner?.Garrisons.Remove(currentUnitGarrison);
            }

            // 교체 및 신규 등록
            var oldSpawn = targetOwnerEntity.Garrisons.FirstOrDefault(g => g.Type == S_GarrisonSlotType.Spawn);
            if (oldSpawn != null)
                targetOwnerEntity.Garrisons.Remove(oldSpawn);

            // 스폰은 0 번 고정 인덱스
            var newSpawn = new EntityGarrisonInfo { SlotIdx = 0 };
            targetOwnerEntity.Garrisons.Add(newSpawn);
            newSpawn.Set(ownerEntityUid, S_GarrisonSlotType.Spawn, spawnUnitUid);

            return await _repo.SaveChangesAsync() ? ERROR_CODE.SUCCESS : ERROR_CODE.FAIL_DATABASE_SAVE;
        }

        public async Task<(ERROR_CODE, S_GarrisonSlotType)> UngarrisonUnitAsync(string id, long ownerEntityUid, long ungarrisonUnitUid)
        {
            var player = await _repo.GetPlayerAsync(id, E_PlayerInclude.PlacedEntities);
            if (player == null)
                return (ERROR_CODE.FAIL_INVALID_USER, S_GarrisonSlotType.None);

            var targetOwnerEntity = player.PlacedEntities.Find(t => t.UID == ownerEntityUid);
            if (targetOwnerEntity == null)
                return (ERROR_CODE.NO_TARGET_ENTITY_FOUND, S_GarrisonSlotType.None);

            // 주둔/스폰 중인지 확인
            var targetGarrison = targetOwnerEntity.Garrisons
                .FirstOrDefault(g => g.EquippedItemUID == ungarrisonUnitUid);

            // 이미 없는 상태면 성공
            if (targetGarrison == null)
                return (ERROR_CODE.SUCCESS, targetGarrison.Type);

            targetOwnerEntity.Garrisons.Remove(targetGarrison);

            var res = await _repo.SaveChangesAsync() ? ERROR_CODE.SUCCESS : ERROR_CODE.FAIL_DATABASE_SAVE;
            var resGarrisonType = res == ERROR_CODE.SUCCESS ? targetGarrison.Type : S_GarrisonSlotType.None;

            return (res, resGarrisonType);
        }

        public async Task<(ERROR_CODE errCode, long uid, int remainedGold, int remainedWood, int remainedFood)> BuyItemAsync(string id, int itemTid)
        {
            var player = await _repo.GetPlayerAsync(id, E_PlayerInclude.InventoryItems);
            if (player == null)
                return (ERROR_CODE.FAIL_INVALID_USER, 0, 0, 0, 0);

            // 아이템 테이블 존재 여부
            if (!_tableService.Container.ItemTable_data.TryGetValue((uint)itemTid, out var itemData))
                return (ERROR_CODE.FAIL_TABLE_DATA_NO_EXIST, 0, 0, 0, 0);

            // 이미 보유 중인지 체크 (같은 TableID의 아이템이 인벤토리에 있으면 구매 불가)
            if (player.InventoryItems.Any(t => t.TableID == itemTid))
                return (ERROR_CODE.ITEM_ALREADY_OWNED, 0, 0, 0, 0);

            var costData = _tableService.Container.PurchaseCostTable_data.Values
                .FirstOrDefault(t => t.RefID == (uint)itemTid);

            if (costData == null)
                return (ERROR_CODE.PURCHASE_COST_NOT_FOUND, 0, 0, 0, 0);

            int costPrice = (int)costData.CostPrice;

            // 재화 처리
            switch (costData.CostCurrencyType)
            {
                case E_CurrencyType.Gold:
                    if (player.Gold < costPrice)
                        return (ERROR_CODE.NOT_ENOUGH_CURRENCY, 0, 0, 0, 0);
                    player.Gold -= costPrice;
                    break;
                case E_CurrencyType.Wood:
                    if (player.Wood < costPrice)
                        return (ERROR_CODE.NOT_ENOUGH_CURRENCY, 0, 0, 0, 0);
                    player.Wood -= costPrice;
                    break;
                case E_CurrencyType.Food:
                    if (player.Food < costPrice)
                        return (ERROR_CODE.NOT_ENOUGH_CURRENCY, 0, 0, 0, 0);
                    player.Food -= costPrice;
                    break;
            }

            // 인벤토리에 아이템 추가
            var newItem = new UserItem()
            {
                OwnerID = id,
                TableID = itemTid,
                Level = 1,
                Quantity = 1
            };

            player.InventoryItems.Add(newItem);

            return await _repo.SaveChangesAsync() ? (ERROR_CODE.SUCCESS, newItem.UID, player.Gold, player.Wood, player.Food) : (ERROR_CODE.FAIL_DATABASE_SAVE, 0, 0, 0, 0);
        }

        public async Task<ERROR_CODE> MoveEntityAsync(string id, long entityUid, float posX, float posZ, float rotY)
        {
            var player = await _repo.GetPlayerAsync(id, E_PlayerInclude.PlacedEntities);
            if (player == null)
                return ERROR_CODE.FAIL_INVALID_USER;

            var entity = player.PlacedEntities.Find(t => t.UID == entityUid);
            if (entity == null)
                return ERROR_CODE.NO_TARGET_ENTITY_FOUND;

            entity.PositionX = posX;
            entity.PositionZ = posZ;
            entity.RotationY = rotY;

            return await _repo.SaveChangesAsync() ? ERROR_CODE.SUCCESS : ERROR_CODE.FAIL_DATABASE_SAVE;
        }

        public async Task<ERROR_CODE> EquipHeroAsync(string id, long heroItemUid)
        {
            var player = await _repo.GetPlayerAsync(id, E_PlayerInclude.InventoryItems);
            if (player == null)
                return ERROR_CODE.FAIL_INVALID_USER;

            // 인벤토리에서 아이템 존재 확인
            var item = player.InventoryItems.Find(t => t.UID == heroItemUid);
            if (item == null)
                return ERROR_CODE.ITEM_NO_EXIST;

            // 아이템이 Hero 타입인지 검증
            if (_tableService.Container.ItemTable_data.TryGetValue((uint)item.TableID, out var itemData) == false)
                return ERROR_CODE.FAIL_TABLE_DATA_NO_EXIST;

            if (itemData.Type != E_ItemType.Hero)
                return ERROR_CODE.INVALID_ITEM;

            // 이미 같은 히어로가 장착 중이면 바로 성공
            if (player.EquippedHeroItemUID == heroItemUid)
                return ERROR_CODE.SUCCESS;

            player.EquippedHeroItemUID = heroItemUid;

            return await _repo.SaveChangesAsync() ? ERROR_CODE.SUCCESS : ERROR_CODE.FAIL_DATABASE_SAVE;
        }

        public async Task<ERROR_CODE> UnequipHeroAsync(string id)
        {
            var player = await _repo.GetPlayerAsync(id);
            if (player == null)
                return ERROR_CODE.FAIL_INVALID_USER;

            // 이미 해제 상태면 바로 성공
            if (player.EquippedHeroItemUID == null)
                return ERROR_CODE.SUCCESS;

            player.EquippedHeroItemUID = null;

            return await _repo.SaveChangesAsync() ? ERROR_CODE.SUCCESS : ERROR_CODE.FAIL_DATABASE_SAVE;
        }

        public async Task<ERROR_CODE> EnterNicknameAsync(string id, string nicknameToEnter)
        {
            if (string.IsNullOrEmpty(nicknameToEnter))
                return (ERROR_CODE.NICKNAME_EMPTY);

            if (nicknameToEnter.Length < 4)
                return (ERROR_CODE.NICKNAME_TOO_SHORT);

            if (nicknameToEnter.Length > 12)
                return (ERROR_CODE.NICKNAME_TOO_LONG);

            if (char.IsDigit(nicknameToEnter[0]))
                return (ERROR_CODE.NICKNAME_START_WITH_DIGIT);

            if (Regex.IsMatch(nicknameToEnter, @"^[a-zA-Z가-힣0-9]*$") == false)
                return ERROR_CODE.NICKNAME_INVALID_CHARACTER;

            // TODO : 비속어 체크
            // if(비속어 버퍼?가져와서 ㄱㄱ)

            // 중복체크
            if (await _repo.IsPlayerExistByNickname(nicknameToEnter))
                return (ERROR_CODE.NICKNAME_DUPLICATE);

            var me = await _repo.GetPlayerAsync(id);

            if (me == null)
                return (ERROR_CODE.FAIL_INVALID_USER);

            // 해당 유저의 닉네임이 이미 설정돼있음(이 패킷 자체가 호출되면 안됨)
            if (string.IsNullOrEmpty(me.Nickname) == false)
                return (ERROR_CODE.NICKNAME_ALREADY_SET);

            me.Nickname = nicknameToEnter;

            return await _repo.SaveChangesAsync() ? ERROR_CODE.SUCCESS : ERROR_CODE.FAIL_DATABASE_SAVE;
        }

        public async Task<ERROR_CODE> SetStatusMessageAsync(string id, string message)
        {
            if (message == null)
                message = string.Empty;

            if (message.Length > 20)
                return ERROR_CODE.INVALID_INPUT;

            var me = await _repo.GetPlayerAsync(id);

            if (me == null)
                return ERROR_CODE.FAIL_INVALID_USER;

            me.StatusMsg = message;

            return await _repo.SaveChangesAsync() ? ERROR_CODE.SUCCESS : ERROR_CODE.FAIL_DATABASE_SAVE;
        }

        public async Task<(ERROR_CODE errCode, PlayerInfo? myInfo)> EnterHomeAsync(string id)
        {
            var me = await _repo.GetPlayerAsync(id, E_PlayerInclude.Full, isReadonly: true);

            if (me == null)
                return (ERROR_CODE.FAIL_INVALID_USER, null);

            // 매칭 필터 리스트에서 제거
            _matchingContextCache.Remove(id);

            return (ERROR_CODE.SUCCESS, me);
        }

        public async Task<(ERROR_CODE errCode, PlayerInfo? myInfo, PlayerInfo? opponentInfo)> SearchOpponentAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return (ERROR_CODE.FAIL_INVALID_USER, null, null);

            // 내 정보 조회
            var me = await _repo.GetPlayerAsync(id, E_PlayerInclude.InventoryItems | E_PlayerInclude.DeploymentSlots);

            if (me == null)
                return (ERROR_CODE.FAIL_INVALID_USER, null, null);

            // 전투 입장료 체크
            int entryCost = TEMP_Data.MatchEntryGold;

            if (me.Gold < entryCost)
                return (ERROR_CODE.NOT_ENOUGH_CURRENCY, null, null);

            var searchRes = await _repo.GetRandomOpponentAsync(askerId: me.ID, _matchingContextCache.GetMatchingListOrdered(me.ID));

            if (searchRes.opponent == null)
                return (ERROR_CODE.SEARCH_OPPONENT_NOT_FOUND, null, null);

            _matchingContextCache.Add(id, searchRes.opponent.ID);

            int remainedGold = Math.Max(me.Gold - entryCost, 0);

            // 재화 차감 적용
            me.Gold = remainedGold;
            _matchingContextCache.Add(me.ID, searchRes.opponent.ID);

            var saveRes = await _repo.SaveChangesAsync() ? ERROR_CODE.SUCCESS : ERROR_CODE.FAIL_DATABASE_SAVE;

            if (saveRes != ERROR_CODE.SUCCESS)
                return (saveRes, null, null);

            return (saveRes, me, searchRes.opponent);
        }

        public async Task<(ERROR_CODE errCode, PlayerInfo? myInfo, PlayerInfo? opponentInfo)> LoadRevengeAsync(
            string id,
            long battleLogUid,
            string opponentId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(opponentId) || battleLogUid <= 0)
                return (ERROR_CODE.INVALID_INPUT, null, null);

            // 내 정보 조회
            var me = await _repo.GetPlayerAsync(id, E_PlayerInclude.DeploymentSlots | E_PlayerInclude.InventoryItems);
            if (me == null)
                return (ERROR_CODE.FAIL_INVALID_USER, null, null);

            // 배틀 로그 조회
            var battleLog = await _repo.GetBattleLogAsync(battleLogUid);
            if (battleLog == null)
                return (ERROR_CODE.BATTLE_LOG_NOT_FOUND, null, null);

            // 방어자가 요청자 본인인지 검증
            if (battleLog.DefenderId != id)
                return (ERROR_CODE.FAIL_INVALID_USER, null, null);

            // 로그의 AttackerId 가 일치하는지 검증
            if (battleLog.AttackerId != opponentId)
                return (ERROR_CODE.FAIL_INVALID_USER, null, null);

            // 이미 복수했는지 검증
            if (battleLog.IsRevenged)
                return (ERROR_CODE.ALREADY_REVENGED, null, null);

            // 전투로그가 최초에 검색으로 생긴건지 검증
            if (battleLog.ModeType != S_BattleModeType.Search)
                return (ERROR_CODE.BATTLE_MODE_INVALID, null, null);

            // 복수 대상 정보 조회
            var opponent = await _repo.GetPlayerAsync(opponentId, E_PlayerInclude.PlacedEntities | E_PlayerInclude.InventoryItems, isReadonly: true);
            if (opponent == null)
                return (ERROR_CODE.SEARCH_OPPONENT_NOT_FOUND, null, null);

            return (ERROR_CODE.SUCCESS, me, opponent);
        }

        async Task<(ERROR_CODE errCode, long uid, int remainedGold, int remainedWood, int remainedFood, StructureTable? structureData)> IPlayerService.CreateStructureAsync(string id, int tableId, float posX, float posZ, float rotY)
        {
            if (_tableService.Container.EntityTable_data.TryGetValue((uint)tableId, out var entityData) == false)
            {
                _logger.LogError($"CreateStructure 잘못된 Input 감지 - 존재하지 않는 Entity 테이블 ID | EntityTID : {tableId}");
                return (ERROR_CODE.NOT_EXIST_IN_TABLE, 0, 0, 0, 0, null);
            }

            if (entityData.EntityType != E_EntityType.Structure)
            {
                _logger.LogError($"CreateStructure 잘못된 Input 감지 - Entity 타입이 Structure 가 아님 | WrongEntityType : {entityData.EntityType}");
                return (ERROR_CODE.INVALID_INPUT, 0, 0, 0, 0, null);
            }

            if (_tableService.Container.StructureTable_data.TryGetValue(entityData.DetailTableID, out var structureData) == false)
            {
                _logger.LogError($"CreateStructure 잘못된 Input 감지 - 해당 StructureData 가 테이블에 존재하지 않음: {entityData.DetailTableID}");
                return (ERROR_CODE.NOT_EXIST_IN_TABLE, 0, 0, 0, 0, null);
            }

            var costData = _tableService.Container.PurchaseCostTable_data.Values
                .FirstOrDefault(t => t.ContentType == E_PurchaseContentType.Entity && t.RefID == (uint)tableId);

            if (costData == null)
            {
                _logger.LogError($"CreateStructure 잘못된 Input 감지 - 해당 TableID 가 PurchaseCost 테이블에 존재하지 않음 : {tableId}");
                return (ERROR_CODE.NOT_EXIST_IN_TABLE, 0, 0, 0, 0, null);
            }

            E_CurrencyType costType = costData.CostCurrencyType;
            int costPrice = (int)costData.CostPrice;

            var result = await _repo.CreateStructure(id, tableId, posX, posZ, rotY, costType, costPrice);

            if (result.errCode != ERROR_CODE.SUCCESS)
                return (result.errCode, 0, 0, 0, 0, null);

            return (ERROR_CODE.SUCCESS, result.uid, result.remainedGold, result.remainedWood, result.remainedFood, structureData);
        }

        async Task<ERROR_CODE> IPlayerService.DestroyStructureAsync(string id, long entityUid)
        {
            var player = await _repo.GetPlayerAsync(id, E_PlayerInclude.PlacedEntities);
            if (player == null)
                return ERROR_CODE.FAIL_INVALID_USER;

            var targetEntity = player.PlacedEntities.FirstOrDefault(t => t.OwnerID == id && t.UID == entityUid);
            if (targetEntity == null)
                return ERROR_CODE.NO_TARGET_ENTITY_FOUND;

            player.PlacedEntities.Remove(targetEntity);

            // DB 에서 확실히 제거 
            _repo.RemoveEntity(targetEntity);

            return await _repo.SaveChangesAsync() ? ERROR_CODE.SUCCESS : ERROR_CODE.FAIL_DATABASE_SAVE;
        }

        public async Task<(ERROR_CODE errCode, PlayerInfo? playerInfo, BattleLogInfo resultLog, long rewardGold, long totalGold, long rewardWood, long totalWood, long rewardFood, long totalFood, int addedBounty, int totalBounty)>
            FinishBattleAsync(string id, string battleSessionId, string opponentId, S_BattleResult battleResult, long[] destroyedEntityUids, float playTime)
        {
            var player = await _repo.GetPlayerAsync(id, E_PlayerInclude.BattleLogs);
            if (player == null)
                return (ERROR_CODE.FAIL_INVALID_USER, null, null, 0, 0, 0, 0, 0, 0, 0, 0);

            // 만약에 현재 메모리로 관리하는 전투 세션에 없다면
            // 제한 시간 Timeout 이 된 후에 Finish 를 보낸 케이스일 것임.
            // 아마 홈버튼눌렀다가 나중에 들어왔던지 뭐 이런 케이스일 확률이 클텐데
            // 이 경우는 이미 비정상적인 상황으로, 시스템 자체에서 ZombieCleaner 에 의해서
            // 삭제되었을 것이므로 (자동 패배 처리) 여기서는 실패 띄우면 됨.
            if (_activeBattleCache.TryPopTargetSession(id, battleSessionId, out var session) == false)
                return (ERROR_CODE.ACTIVE_BATTLE_SESSION_NOT_EXIST, null, null, 0, 0, 0, 0, 0, 0, 0, 0);

            var opponent = await _repo.GetPlayerAsync(opponentId);

            if (opponent == null)
            {
                _logger.LogError($"전투 결과 처리중 Opponent 유저의 ID 를 찾는데 실패하였습니다. | 침공자 ID : {id}, Opponent ID : {opponentId}");
                return (ERROR_CODE.FAIL_INVALID_USER, null, null, 0, 0, 0, 0, 0, 0, 0, 0);
            }

            // 기본 보상 계산 (결과에 따라)
            int baseGold, baseWood, baseFood, baseBounty;
            switch (battleResult)
            {
                case S_BattleResult.Win:
                    baseGold = TEMP_Data.WinGold;
                    baseWood = TEMP_Data.WinWood;
                    baseFood = TEMP_Data.WinFood;
                    baseBounty = TEMP_Data.WinBounty;
                    break;
                case S_BattleResult.Lose:
                    baseGold = TEMP_Data.LoseGold;
                    baseWood = TEMP_Data.LoseWood;
                    baseFood = TEMP_Data.LoseFood;
                    baseBounty = TEMP_Data.LoseBounty;
                    break;
                case S_BattleResult.Draw:
                    baseGold = TEMP_Data.DrawGold;
                    baseWood = TEMP_Data.DrawWood;
                    baseFood = TEMP_Data.DrawFood;
                    baseBounty = TEMP_Data.DrawBounty;
                    break;
                default:
                    return (ERROR_CODE.INVALID_INPUT, null, null, 0, 0, 0, 0, 0, 0, 0, 0);
            }

            // 파괴한 건물 수에 비례한 추가 골드
            var destroyedEntityUidSet = new HashSet<long>(destroyedEntityUids?.Length ?? 0);
            if (destroyedEntityUids != null)
            {
                for (int i = 0; i < destroyedEntityUids.Length; i++)
                {
                    destroyedEntityUidSet.Add(destroyedEntityUids[i]);
                }
            }

            int destroyedCount = 0;

            if (destroyedEntityUidSet.Count > 0)
            {
                // 실제로 적 플레이어가 가지고 있는 Entity 인지
                // 검증한 후에 카운트 계산
                foreach (var entity in opponent.PlacedEntities)
                {
                    if (destroyedEntityUidSet.Contains(entity.UID))
                    {
                        if (entity.NeedsRepair == false)
                            entity.NeedsRepair = true;

                        destroyedCount++;
                    }
                }
            }

            int bonusGold = destroyedCount * TEMP_Data.GoldPerDestroyedEntity;

            int rewardGold = baseGold + bonusGold;
            int rewardWood = baseWood;
            int rewardFood = baseFood;
            int addedBounty = baseBounty;

            // 재화 처리 
            player.Gold += rewardGold;
            player.Wood += rewardWood;
            player.Food += rewardFood;

            // 현상금 처리
            player.Bounty += addedBounty;

            if (player.Bounty < 0)
                player.Bounty = 0;

            try
            {
                var resLog = await AddBattleLog(
                    session.SessionId,
                    session.AttackerId,
                    session.AttackerNickname,
                    session.DefenderId,
                    session.DefenderNickname,
                    DateTime.UtcNow,
                    battleResult,
                    rewardGold,
                    rewardWood,
                    rewardFood,
                    session.ModeType,
                    dbSave: false);

                if (resLog.errCode != ERROR_CODE.SUCCESS)
                    return (ERROR_CODE.FAIL_DATABASE_SAVE, null, null, 0, 0, 0, 0, 0, 0, 0, 0);

                var res = await _repo.SaveChangesAsync();

                if (res == false)
                    return (ERROR_CODE.FAIL_DATABASE_SAVE, null, null, 0, 0, 0, 0, 0, 0, 0, 0);

                return (ERROR_CODE.SUCCESS, player, resLog.resultLog, rewardGold, player.Gold, rewardWood, player.Wood, rewardFood, player.Food, addedBounty, player.Bounty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"FinishBattle 중 치명적 에러: {ex}");
                return (ERROR_CODE.EXCEPTION, null, null, 0, 0, 0, 0, 0, 0, 0, 0);
            }
        }

        public async Task<(ERROR_CODE errCode, PlayerInfo? playerInfo, long[] repairedEntityUids)>
            RepairEntitiesAsync(string id, long[] targetEntityUids, long expectedGold, long expectedWood, long expectedFood)
        {
            if (targetEntityUids == null || targetEntityUids.Length == 0)
                return (ERROR_CODE.NO_TARGET_ENTITY_FOUND, null, null);

            var player = await _repo.GetPlayerAsync(id, E_PlayerInclude.PlacedEntities);

            if (player == null || player.PlacedEntities == null)
                return (ERROR_CODE.FAIL_INVALID_USER, null, null);

            var targetSet = new HashSet<long>(targetEntityUids);
            var toRepair = new List<EntityItemInfo>();

            // 유저의 소유 건물 중 요청 UID와 일치하며 수리가 필요한 것 필터
            foreach (var entity in player.PlacedEntities)
            {
                if (entity.NeedsRepair)
                {
                    if (targetSet.Contains(entity.UID))
                    {
                        toRepair.Add(entity);
                    }
                    else
                    {
                        // 서버에선 수리해야하는 상태인데 클라 데이터인 targetSet 에 없다면 싱크 어긋난 상태임
                        return (ERROR_CODE.ENTITY_UNSYNC, null, null);
                    }
                }
            }

            if (toRepair.Count != targetSet.Count)
                return (ERROR_CODE.NO_TARGET_ENTITY_FOUND, null, null);

            // 총 필요 비용 산출 (현재는 임시로 일괄 50, 10, 10 사용)
            long totalCostGold = (long)toRepair.Count * TEMP_Data.RepairCostGold;
            long totalCostWood = (long)toRepair.Count * TEMP_Data.RepairCostWood;
            long totalCostFood = (long)toRepair.Count * TEMP_Data.RepairCostFood;

            // 클라이언트가 예상한 비용이 다르면 실패 (가격 정보 불일치 방어)
            if (totalCostGold != expectedGold || totalCostWood != expectedWood || totalCostFood != expectedFood)
            {
                _logger.LogError($@"
실제 수리 비용과 클라로부터 받아온 기대 비용이 서로 다릅니다.
(totalCostGold : {totalCostGold}, expectedGold : {expectedGold}),
(totalCostWood : {totalCostWood}, expectedWood : {expectedWood}),
(totalCostFood : {totalCostFood}, expectedFood : {expectedFood})");

                return (ERROR_CODE.PRICE_CHANGED, null, null);
            }

            // 재화 검증
            if (player.Gold < totalCostGold || player.Wood < totalCostWood || player.Food < totalCostFood)
            {
                return (ERROR_CODE.NOT_ENOUGH_CURRENCY, null, null);
            }

            // 수리 재화 처리
            player.Gold -= (int)totalCostGold;
            player.Wood -= (int)totalCostWood;
            player.Food -= (int)totalCostFood;

            for (int i = 0; i < toRepair.Count; i++)
            {
                toRepair[i].NeedsRepair = false;
            }

            var res = await _repo.SaveChangesAsync();
            if (res == false)
                return (ERROR_CODE.FAIL_DATABASE_SAVE, null, null);

            return (ERROR_CODE.SUCCESS, player, targetEntityUids.ToArray());
        }

        //------------------------------------------------------//

        private static readonly string[] AvailableMaps = { "MainMap01", "SubMap02" };

        string GetRandomMap()
        {
            return AvailableMaps[new Random().Next(AvailableMaps.Length)];
        }

        private List<EntityItemInfo> DefaultEntities(string userId)
        {
            return new List<EntityItemInfo>()
            {
                new EntityItemInfo()
                {
                    TableID = 156, Level = 1, OwnerID = userId,
                    PositionX = 33.3f, PositionZ = 34.6f, RotationY = -90,
                },
                new EntityItemInfo()
                {
                    TableID = 163, Level = 1, OwnerID = userId,
                    PositionX = 52.5f, PositionZ = 34.7f, RotationY = -180
                },
                new EntityItemInfo()
                {
                    TableID = 157, Level = 1, OwnerID = userId,
                    PositionX = 63.3f, PositionZ = 33.5f, RotationY = -180
                },
                new EntityItemInfo()
                {
                    TableID = 372, Level = 1, OwnerID = userId,
                    PositionX = 38f, PositionZ = 47f, RotationY = -90
                },
                new EntityItemInfo()
                {
                    TableID = 144, Level = 1, OwnerID = userId,
                    PositionX = 51.32f, PositionZ = 58.56f, RotationY = -180
                },
                new EntityItemInfo()
                {
                    TableID = 145, Level = 1, OwnerID = userId,
                    PositionX = 61.04f, PositionZ = 57.6f, RotationY = -180
                },
            };
        }

        private List<UserItem> DefaultItems(string accountID, UserItem initialHeroItem)
        {
            return new List<UserItem>()
            {
                initialHeroItem,
                new UserItem()
                {
                    OwnerID = accountID,
                    Level=1,
                    TableID = 1,
                    Quantity = 1,
                },
                new UserItem()
                {
                    OwnerID = accountID,
                    Level=1,
                    TableID = 2,
                    Quantity = 1,
                },
                new UserItem()
                {
                    OwnerID = accountID,
                    Level = 1,
                    TableID = 3,
                    Quantity = 1,
                },
                new UserItem()
                {
                    OwnerID = accountID,
                    Level = 1,
                    TableID = 4,
                    Quantity = 1,
                },new UserItem()
                {
                    OwnerID = accountID,
                    Level = 1,
                    TableID = 5,
                    Quantity = 1,
                },
            };
        }

        private List<DeploymentSlot> DefaultDeploymentSlots(string ownerId, List<UserItem> userItems)
        {
            var deploymentSlots = new List<DeploymentSlot>()
            {
                new DeploymentSlot()
                {
                     OwnerID = ownerId,
                     SlotIdx = 0,
                },new DeploymentSlot()
                {
                     OwnerID = ownerId,
                     SlotIdx = 1,
                }
                ,new DeploymentSlot()
                {
                     OwnerID = ownerId,
                     SlotIdx = 2,
                }
                ,new DeploymentSlot()
                {
                     OwnerID = ownerId,
                     SlotIdx = 3,
                }
                ,new DeploymentSlot()
                {
                     OwnerID = ownerId,
                     SlotIdx = 4,
                },new DeploymentSlot()
                {
                     OwnerID = ownerId,
                     SlotIdx = 5,
                },new DeploymentSlot()
                {
                     OwnerID = ownerId,
                     SlotIdx = 6,
                },new DeploymentSlot()
                {
                     OwnerID = ownerId,
                     SlotIdx = 7,
                },new DeploymentSlot()
                {
                     OwnerID = ownerId,
                     SlotIdx = 8,
                }
            };

            var squadItems = userItems.Where(x =>
                _tableService.Container.ItemTable_data.TryGetValue((uint)x.TableID, out var data) &&
                data.Type == E_ItemType.Squad).ToList();

            for (int i = 0; i < squadItems.Count && i < deploymentSlots.Count; i++)
            {
                deploymentSlots[i].EquippedItem = squadItems[i];
            }

            return deploymentSlots;
        }

        async Task<ERROR_CODE> IPlayerService.ChangeSkill(string id, int[] skillSet, int[] spellSet)
        {
            var player = await _repo.GetPlayerAsync(id);
            if (player == null)
                return ERROR_CODE.FAIL_INVALID_USER;

            player.SkillID01 = skillSet[0];
            player.SkillID02 = skillSet[1];
            player.SkillID03 = skillSet[2];

            player.SpellID01 = spellSet[0];
            player.SpellID02 = spellSet[1];
            player.SpellID03 = spellSet[2];

            return await _repo.SaveChangesAsync() ? ERROR_CODE.SUCCESS : ERROR_CODE.FAIL_DATABASE_SAVE;
        }

        public async Task<(ERROR_CODE errCode, BattleLogInfo resultLog)> AddBattleLog(
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
            bool dbSave = true)
        {
            if (string.IsNullOrEmpty(attackerId) || string.IsNullOrEmpty(defenderId))
                return (ERROR_CODE.FAIL_INVALID_USER, null);

            var resLog = _repo.AddBattleLog(sessionId, attackerId, attackerNickname, defenderId, defenderNickname, timeUtc, result, lootedGold, lootedWood, lootedFood, modeType);

            if (dbSave == false)
                return (ERROR_CODE.SUCCESS, resLog);

            return await _repo.SaveChangesAsync() ? (ERROR_CODE.SUCCESS, resLog) : (ERROR_CODE.FAIL_DATABASE_SAVE, null);
        }

        public async Task<(ERROR_CODE errCode, string generatedSessionId)> StartBattle(string id, string opponentPlayerId, S_BattleModeType modeType, long targetBattleLogUid = 0)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(opponentPlayerId))
                return (ERROR_CODE.FAIL_INVALID_USER, string.Empty);

            var playerRes = await _repo.IsPlayerExistAndGetNicknameByIDAsync(id);
            if (playerRes.res == false)
                return (ERROR_CODE.FAIL_INVALID_USER, string.Empty);

            var opponentPlayer = await _repo.GetPlayerAsync(opponentPlayerId, isReadonly: true);
            if (opponentPlayer == null)
                return (ERROR_CODE.FAIL_INVALID_USER, string.Empty);

            // 복수 모드: StartBattle 확정 시점에 IsRevenged = true 처리 (복수권 즉시 소진)
            if (modeType == S_BattleModeType.Revenge && targetBattleLogUid > 0)
            {
                var battleLog = await _repo.GetBattleLogAsync(targetBattleLogUid);

                if (battleLog == null || battleLog.DefenderId != id)
                    return (ERROR_CODE.BATTLE_LOG_NOT_FOUND, string.Empty);

                if (battleLog.IsRevenged)
                    return (ERROR_CODE.ALREADY_REVENGED, string.Empty);

                battleLog.IsRevenged = true;

                bool saveRes = await _repo.SaveChangesAsync();
                if (saveRes == false)
                    return (ERROR_CODE.FAIL_DATABASE_SAVE, string.Empty);
            }

            string newSessionId = Guid.NewGuid().ToString("N");

            var registerRes = _activeBattleCache.TryRegister(newSessionId, id, playerRes.nickname, opponentPlayerId, opponentPlayer.Nickname, modeType);
            if (registerRes != ERROR_CODE.SUCCESS)
                return (registerRes, string.Empty);

            return (ERROR_CODE.SUCCESS, newSessionId);
        }
    }
}
