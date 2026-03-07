using GameDB;
using JNetwork;
using LearningServer01.Data;
using LearningServer01.Repositories;
using LearningServer01.Services.TableService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using LearningServer01.Services.AuthService;

namespace LearningServer01.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;
        private readonly IAuthService _authService;
        private readonly IPlayerRepository _repository;
        private readonly ILockService _lockService;
        private readonly ITableService _tableService;
        private readonly ILogger<PlayerController> _logger;
        private readonly GameSettings _gameSettings;

        string? GetUserID() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public PlayerController(
            ILogger<PlayerController> logger,
            IPlayerService playerService,
            IAuthService authService,
            IPlayerRepository repository,
            ILockService lockService,
            IOptions<GameSettings> options,
            ITableService tableService)
        {
            _logger = logger;
            _playerService = playerService;
            _authService = authService;
            _repository = repository;
            _lockService = lockService;
            _tableService = tableService;
            _gameSettings = options.Value;
        }

        [HttpPost(nameof(CheckVersion))]
        [AllowAnonymous]
        public async Task<Res_CheckVersion> CheckVersion([FromBody] Req_CheckVersion req)
        {
            if (req == null)
                return new Res_CheckVersion() { Result = ERROR_CODE.FAIL_EMPTY_REQUEST };

            // TODO : Service 레이어에서 구현할 것
            // 근데 어떤 Service 를 추가해야하지 이런건?
            // 관리자같은 느낌인데 
            var res = new Res_CheckVersion();
            var validAppVersion = _gameSettings.AppVersion;

            if (req.ClientAppVersion != validAppVersion)
            {
                res.Result = ERROR_CODE.FAIL_INVALID_APP_VERSION;
                res.Message = "앱 재다운로드 필요";

                // TODO : 나중에는 store url 넣어줘야할듯 
                // if(req.Platform)
                res.RedirectStoreUrl = "store_url";
                return res;
            }

            bool isMaintenance = _gameSettings.IsMaintenance;
            if (isMaintenance)
            {
                res.Result = ERROR_CODE.FAIL_MAINTENANCE;
                // message 도 appsettings 에 넣어야할지는 고민 
                res.Message = $"서버 점검 (n시 종료)";
                return res;
            }

            res.Result = ERROR_CODE.SUCCESS;

            res.CdnBaseUrl = _gameSettings.CdnBaseUrl;
            res.LatestAppVersion = validAppVersion;
            res.TableMetadataHash = _tableService.Metadata.TotalHash;

            return res;
        }

        [HttpPost(nameof(Register))]
        [AllowAnonymous]
        public async Task<Res_RegisterAccount> Register([FromBody] Req_RegisterAccount req)
        {
            if (req == null)
                return new Res_RegisterAccount() { Result = ERROR_CODE.FAIL_EMPTY_REQUEST };

            if (string.IsNullOrEmpty(req.AccountID) || string.IsNullOrEmpty(req.Password))
                return new Res_RegisterAccount() { Result = ERROR_CODE.REGISTER_FAIL_INVALID };

            // 신규회원 기본 세팅값
            var sres = await _playerService.RegisterNewPlayerAsync(req.AccountID, req.Password);

            if (sres.errCode != ERROR_CODE.SUCCESS)
                return new Res_RegisterAccount() { Result = sres.errCode };

            return new Res_RegisterAccount() { Result = ERROR_CODE.SUCCESS };
        }

        //[HttpGet("{userId}")]
        //public async Task<ActionResult<PlayerInfo>> Get(string userId)
        //{
        //    var info = await _repository.GetPlayer(userId);

        //    if (info == null)
        //        return NotFound($"{userId}님은 존재하지 않습니다.");

        //    return Ok(info);
        //}

        [HttpPost(nameof(Login))]
        [AllowAnonymous]
        public async Task<Res_Login> Login([FromBody] Req_Login req)
        {
            _logger.LogInformation(nameof(Login));

            _logger.LogInformation($"★ Login 요청 수신 | AccountID=[{req?.AccountID}], Password=[{(req?.Password != null ? "있음" : "null")}]");

            if (req == null)
                return new Res_Login() { Result = ERROR_CODE.FAIL_EMPTY_REQUEST };

            var user = await _repository.GetPlayerFullAsync(req.AccountID);

            if (user == null)
                return new Res_Login() { Result = ERROR_CODE.LOGIN_FAIL_USER_NOT_EXIST };

            bool isValid = _authService.VerifyPassword(req.Password, user.Password);

            if (isValid == false)
                return new Res_Login() { Result = ERROR_CODE.LOGIN_PW_WRONG };

            var authToken = _authService.CreateToken(user.ID);

            return user.ToLoginResponse(authToken, _tableService);
        }

        [HttpPost(nameof(EnterHome))]
        [Authorize]
        public async Task<Res_EnterHome> EnterHome([FromBody] Req_EnterHome req)
        {
            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
                return new Res_EnterHome() { Result = ERROR_CODE.FAIL_INVALID_USER };

            var player = await _repository.GetPlayerFullAsync(userId);

            if (player == null)
                return new Res_EnterHome() { Result = ERROR_CODE.FAIL_INVALID_USER };

            return player.ToEnterHomeResponse(_tableService);
        }

        [HttpPost(nameof(SearchOpponent))]
        [Authorize]
        public async Task<Res_SearchOpponent> SearchOpponent([FromBody] Req_SearchOpponent req)
        {
            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
                return new Res_SearchOpponent() { Result = ERROR_CODE.FAIL_INVALID_USER };

            // 내 정보 조회
            var me = await _repository.GetPlayerFullAsync(userId);
            if (me == null)
                return new Res_SearchOpponent() { Result = ERROR_CODE.FAIL_INVALID_USER };

            var searchRes = await _playerService.SearchOpponentAsync(me);

            if (searchRes.errCode != ERROR_CODE.SUCCESS)
                return new Res_SearchOpponent() { Result = searchRes.errCode };

            var opponent = searchRes.opponent;

            return me.ToSearchOpponentResponse(opponent, _tableService);
        }

#if DEBUG
        [HttpPost(nameof(CheatAddCurrency))]
        [Authorize]
        public async Task<Res_CheatAddCurrency> CheatAddCurrency([FromBody] Req_CheatAddCurrency req)
        {
            if (req == null)
                return new Res_CheatAddCurrency() { Result = ERROR_CODE.FAIL_EMPTY_REQUEST };

            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
                return new Res_CheatAddCurrency() { Result = ERROR_CODE.FAIL_UNKNOWN };

            if (req.Amount == 0)
                return new Res_CheatAddCurrency() { Result = ERROR_CODE.INVALID_INPUT };

            var sres = await _playerService.CheatCurrency(userId, req.CurrencyType);

            if (sres.errCode != ERROR_CODE.SUCCESS)
                return new Res_CheatAddCurrency() { Result = sres.errCode };

            return new Res_CheatAddCurrency() { Result = ERROR_CODE.SUCCESS, CurrentAmount = sres.remainedCurrency };
        }
#endif

        [HttpPost(nameof(EquipDeploymentSlot))]
        [Authorize]
        public async Task<Res_EquipDeploymentSlot> EquipDeploymentSlot(Req_EquipDeploymentSlot req)
        {
            if (req == null)
                return new Res_EquipDeploymentSlot() { Result = ERROR_CODE.FAIL_EMPTY_REQUEST };

            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
                return new Res_EquipDeploymentSlot() { Result = ERROR_CODE.FAIL_INVALID_USER };

            var sres = await _playerService.EquipDeploymentSlotAsync(userId, req.EquipSlotIdx, req.EquipItemUid);

            return new Res_EquipDeploymentSlot() { Result = sres };
        }

        [HttpPost(nameof(UnequipDeploymentSlot))]
        [Authorize]
        public async Task<Res_UnequipDeploymentSlot> UnequipDeploymentSlot(Req_UnequipDeploymentSlot req)
        {
            if (req == null)
                return new Res_UnequipDeploymentSlot() { Result = ERROR_CODE.FAIL_EMPTY_REQUEST };

            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
                return new Res_UnequipDeploymentSlot() { Result = ERROR_CODE.FAIL_INVALID_USER };

            var sres = await _playerService.UnequipDeploymentSlotAsync(userId, req.SlotIdx);

            return new Res_UnequipDeploymentSlot() { Result = sres };
        }

        [HttpPost(nameof(ChangeSkill))]
        [Authorize]
        public async Task<Res_ChangeSkill> ChangeSkill(Req_ChangeSkill req)
        {
            if (req == null)
                return new Res_ChangeSkill() { Result = ERROR_CODE.FAIL_EMPTY_REQUEST };

            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
                return new Res_ChangeSkill() { Result = ERROR_CODE.FAIL_INVALID_USER };

            var sres = await _playerService.ChangeSkill(userId, req.SkillSet, req.SpellSet);

            return new Res_ChangeSkill() { Result = sres };
        }

        [HttpPost(nameof(CreateStructure))]
        [Authorize]
        public async Task<Res_CreateStructure> CreateStructure(Req_CreateStructure req)
        {
            if (req == null)
                return new Res_CreateStructure() { Result = ERROR_CODE.FAIL_EMPTY_REQUEST };

            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
                return new Res_CreateStructure() { Result = ERROR_CODE.FAIL_INVALID_USER };

            var result = await _playerService.CreateStructureAsync(userId, req.TableID, req.PositionX, req.PositionZ, req.RotationY);

            return result.ToCreateStructureResponse(_tableService);
        }

        [HttpPost(nameof(DestroyStructure))]
        [Authorize]
        public async Task<Res_DestroyStructure> DestroyStructure(Req_DestroyStructure req)
        {
            if (req == null)
                return new Res_DestroyStructure() { Result = ERROR_CODE.FAIL_EMPTY_REQUEST };

            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
                return new Res_DestroyStructure() { Result = ERROR_CODE.FAIL_INVALID_USER };

            var sres = await _playerService.DestroyStructureAsync(userId, req.UID);

            return new Res_DestroyStructure() { Result = sres };
        }

        [HttpPost(nameof(GarrisonUnit))]
        [Authorize]
        public async Task<Res_GarrisonUnit> GarrisonUnit(Req_GarrisonUnit req)
        {
            var res = new Res_GarrisonUnit();

            if (req == null)
            {
                res.Result = ERROR_CODE.FAIL_EMPTY_REQUEST;
                return res;
            }

            var userId = GetUserID();
            if (string.IsNullOrEmpty(userId))
            {
                res.Result = ERROR_CODE.FAIL_INVALID_USER;
                return res;
            }

            res.Result = await _playerService.GarrisonUnitAsync(userId, req.OwnerEntityUid, req.SlotIdx, req.GarrisonUnitUid);

            return res;
        }

        [HttpPost(nameof(SetSpawnUnit))]
        [Authorize]
        public async Task<Res_SetSpawnUnit> SetSpawnUnit(Req_SetSpawnUnit req)
        {
            var res = new Res_SetSpawnUnit();

            if (req == null)
            {
                res.Result = ERROR_CODE.FAIL_EMPTY_REQUEST;
                return res;
            }

            var userId = GetUserID();
            if (string.IsNullOrEmpty(userId))
            {
                res.Result = ERROR_CODE.FAIL_INVALID_USER;
                return res;
            }

            res.Result = await _playerService.SetSpawnUnitAsync(userId, req.OwnerEntityUid, req.SpawnUnitUid);

            return res;
        }

        [HttpPost(nameof(UngarrisonUnit))]
        [Authorize]
        public async Task<Res_UngarrisonUnit> UngarrisonUnit(Req_UngarrisonUnit req)
        {
            var res = new Res_UngarrisonUnit();

            if (req == null)
            {
                res.Result = ERROR_CODE.FAIL_EMPTY_REQUEST;
                return res;
            }

            var userId = GetUserID();
            if (string.IsNullOrEmpty(userId))
            {
                res.Result = ERROR_CODE.FAIL_INVALID_USER;
                return res;
            }

            var processResult = await _playerService.UngarrisonUnitAsync(userId, req.OwnerEntityUid, req.UngarrisonUnitUid);

            res.Result = processResult.Item1;
            res.OperatedGarrisonType = processResult.Item2;

            return res;
        }

        [HttpPost(nameof(BuyItem))]
        [Authorize]
        public async Task<Res_BuyItem> BuyItem(Req_BuyItem req)
        {
            var res = new Res_BuyItem();

            if (req == null)
            {
                res.Result = ERROR_CODE.FAIL_EMPTY_REQUEST;
                return res;
            }

            var userId = GetUserID();
            if (string.IsNullOrEmpty(userId))
            {
                res.Result = ERROR_CODE.FAIL_INVALID_USER;
                return res;
            }

            var result = await _playerService.BuyItemAsync(userId, req.ItemTid);
            res.Result = result.errCode;

            if (result.errCode == ERROR_CODE.SUCCESS)
            {
                res.UID = result.uid;
                res.RemainedGold = result.remainedGold;
                res.RemainedWood = result.remainedWood;
                res.RemainedFood = result.remainedFood;
            }

            return res;
        }

        [HttpPost(nameof(MoveEntity))]
        [Authorize]
        public async Task<Res_MoveEntity> MoveEntity(Req_MoveEntity req)
        {
            var res = new Res_MoveEntity();

            if (req == null)
            {
                res.Result = ERROR_CODE.FAIL_EMPTY_REQUEST;
                return res;
            }

            var userId = GetUserID();
            if (string.IsNullOrEmpty(userId))
            {
                res.Result = ERROR_CODE.FAIL_INVALID_USER;
                return res;
            }

            res.Result = await _playerService.MoveEntityAsync(userId, req.MoveEntityUid, req.PositionX, req.PositionZ, req.RotationY);

            return res;
        }

        [HttpPost(nameof(EquipHero))]
        [Authorize]
        public async Task<Res_EquipHero> EquipHero(Req_EquipHero req)
        {
            var res = new Res_EquipHero();

            if (req == null)
            {
                res.Result = ERROR_CODE.FAIL_EMPTY_REQUEST;
                return res;
            }

            var userId = GetUserID();
            if (string.IsNullOrEmpty(userId))
            {
                res.Result = ERROR_CODE.FAIL_INVALID_USER;
                return res;
            }

            res.Result = await _playerService.EquipHeroAsync(userId, req.HeroItemUid);

            return res;
        }

        [HttpPost(nameof(UnequipHero))]
        [Authorize]
        public async Task<Res_UnequipHero> UnequipHero(Req_UnequipHero req)
        {
            var res = new Res_UnequipHero();

            if (req == null)
            {
                res.Result = ERROR_CODE.FAIL_EMPTY_REQUEST;
                return res;
            }

            var userId = GetUserID();
            if (string.IsNullOrEmpty(userId))
            {
                res.Result = ERROR_CODE.FAIL_INVALID_USER;
                return res;
            }

            res.Result = await _playerService.UnequipHeroAsync(userId);

            return res;
        }

        //----------------------------------------------------------------------------//

        StructureSpecificData ToStructureSpecificData(StructureTable tableData)
        {
            return ToStructureSpecificData(null, null, tableData);
        }

        StructureSpecificData ToStructureSpecificData(List<EntityGarrisonInfo> garrisons, List<UserItem> items, StructureTable tableData)
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
                        _logger.LogError($"[ERROR] Garrison 타입이 None 이 될 수 없음");
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
                        _tableService.Container.ItemTable_data.TryGetValue((uint)targetGarrisonedItem.TableID, out itemData) &&
                        itemData.Type == E_ItemType.Squad &&
                        _tableService.Container.EntityTable_data.TryGetValue(itemData.RefID, out entityData) &&
                        entityData.EntityType == E_EntityType.Character &&
                        _tableService.Container.CharacterTable_data.ContainsKey(entityData.DetailTableID);

                    if (isValid == false)
                    {
                        // 만약 targetGarrisonedItem 이 Null 이라면 Garrison 주둔군 데이터는 존재하나
                        // 실제로 이 주둔군 Item 의 UID 가 ItemList에 없는 케이스 - 데이터 싱크 깨짐 의심
                        _logger.LogError($"[ERROR] SpecificData 조립1 검증 데이터 오류 | IsNull : {g == null} , UID : {g.UID} , EquippedItemUID : {g.EquippedItemUID} , OwnerEntityUID : {g.OwnerEntityUID} , GarrisonedItemIsNull : {targetGarrisonedItem == null}");
                        continue;
                    }

                    if (g.Type == S_GarrisonSlotType.Garrison)
                    {
                        if (g.SlotIdx < 0 || g.SlotIdx >= garrisonCount)
                        {
                            _logger.LogError($"[ERROR] SpecificData 조립2 검증 데이터 오류 | SlotIdx 의 인덱스 범위 벗어남 | Idx : {g.SlotIdx} , MaxCount : {garrisonCount}");
                            continue;
                        }

                        if (data.GarrisonedItemUIDs[g.SlotIdx] != 0)
                        {
                            // 루프중 설정이 된게 또 설정 시도하려고 하고있음. 인풋 데이터에 중복 체크
                            _logger.LogError($"[ERROR] SpecificData 조립3 검증 데이터 오류 | 인풋 주둔 데이터의 SlotIdx 중복 에러");
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
                            _logger.LogError($"[ERROR] SpecificData 조립4 검증 데이터 오류 | 인풋 주둔 데이터의 SlotIdx 중복 에러");
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
