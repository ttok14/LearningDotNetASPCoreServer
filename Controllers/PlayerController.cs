using JNetwork;
using LearningServer01.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using LearningServer01.Services.AuthService;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
        private readonly ITableService _tableService;
        private readonly GameSettings _gameSettings;

        string? GetUserID() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public PlayerController(
            IPlayerService playerService,
            IAuthService authService,
            IPlayerRepository repository,
            IOptions<GameSettings> options,
            ITableService tableService)
        {
            _playerService = playerService;
            _authService = authService;
            _repository = repository;
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

        [HttpPost(nameof(Login))]
        [AllowAnonymous]
        public async Task<Res_Login> Login([FromBody] Req_Login req)
        {
            if (req == null)
                return new Res_Login() { Result = ERROR_CODE.FAIL_EMPTY_REQUEST };

            var authRes = await _authService.LoginAsync(req.AccountID, req.Password);

            if (authRes.errCode != ERROR_CODE.SUCCESS)
                return new Res_Login() { Result = authRes.errCode };

            var playerRes = await _playerService.PostLoginAsync(authRes.loggedInPlayerInfo);
            if (playerRes != ERROR_CODE.SUCCESS)
                return new() { Result = playerRes };

            return authRes.loggedInPlayerInfo.ToLoginResponse(authRes.token, _tableService);
        }

        [HttpPost(nameof(EnterNickname))]
        [Authorize]
        public async Task<Res_EnterNickname> EnterNickname([FromBody] Req_EnterNickname req)
        {
            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
                return new Res_EnterNickname() { Result = ERROR_CODE.FAIL_INVALID_USER };

            var sres = await _playerService.EnterNicknameAsync(userId, req.Nickname);

            return new Res_EnterNickname() { Result = sres };
        }

        [HttpPost(nameof(SetStatusMessage))]
        [Authorize]
        public async Task<Res_SetStatusMessage> SetStatusMessage([FromBody] Req_SetStatusMessage req)
        {
            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
                return new Res_SetStatusMessage() { Result = ERROR_CODE.FAIL_INVALID_USER };

            var sres = await _playerService.SetStatusMessageAsync(userId, req.Message);

            return new Res_SetStatusMessage() { Result = sres };
        }

        [HttpPost(nameof(EnterHome))]
        [Authorize]
        public async Task<Res_EnterHome> EnterHome([FromBody] Req_EnterHome req)
        {
            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
                return new Res_EnterHome() { Result = ERROR_CODE.FAIL_INVALID_USER };

            var sres = await _playerService.EnterHomeAsync(userId);

            if (sres.errCode != ERROR_CODE.SUCCESS)
                return new Res_EnterHome() { Result = sres.errCode };

            return sres.myInfo.ToEnterHomeResponse(_tableService);
        }

        [HttpPost(nameof(SearchOpponent))]
        [Authorize]
        public async Task<Res_SearchOpponent> SearchOpponent([FromBody] Req_SearchOpponent req)
        {
            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
                return new Res_SearchOpponent() { Result = ERROR_CODE.FAIL_INVALID_USER };

            var sres = await _playerService.SearchOpponentAsync(userId);

            if (sres.errCode != ERROR_CODE.SUCCESS)
                return new Res_SearchOpponent() { Result = sres.errCode };

            var opponent = sres.opponentInfo;

            return sres.myInfo.ToSearchOpponentResponse(opponent, _tableService);
        }

        [HttpPost(nameof(LoadRevenge))]
        [Authorize]
        public async Task<Res_LoadRevenge> LoadRevenge([FromBody] Req_LoadRevenge req)
        {
            if (req == null)
                return new Res_LoadRevenge() { Result = ERROR_CODE.FAIL_EMPTY_REQUEST };

            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
                return new Res_LoadRevenge() { Result = ERROR_CODE.FAIL_INVALID_USER };

            var sres = await _playerService.LoadRevengeAsync(userId, req.BattleLogUid, req.OpponentId);

            if (sres.errCode != ERROR_CODE.SUCCESS)
                return new Res_LoadRevenge() { Result = sres.errCode };

            return sres.myInfo.ToLoadRevengeResponse(sres.opponentInfo, _tableService);
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
        public async Task<Res_EquipDeploymentSlot> EquipDeploymentSlot([FromBody] Req_EquipDeploymentSlot req)
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

        [HttpPost(nameof(StartBattle))]
        [Authorize]
        public async Task<Res_StartBattle> StartBattle([FromBody] Req_StartBattle req)
        {
            if (req == null)
                return new() { Result = ERROR_CODE.FAIL_EMPTY_REQUEST };

            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
                return new() { Result = ERROR_CODE.FAIL_INVALID_USER };

            var sres = await _playerService.StartBattle(userId, req.OpponentPlayerId, req.ModeType, req.TargetBattleLogUid);

            if (sres.errCode != ERROR_CODE.SUCCESS)
                return new() { Result = sres.errCode };

            var res = new Res_StartBattle();

            res.Result = ERROR_CODE.SUCCESS;
            res.SessionId = sres.generatedSessionId;

            return res;
        }

        [HttpPost(nameof(FinishBattle))]
        [Authorize]
        public async Task<Res_FinishBattle> FinishBattle([FromBody] Req_FinishBattle req)
        {
            if (req == null)
                return new Res_FinishBattle() { Result = ERROR_CODE.FAIL_EMPTY_REQUEST };

            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
                return new Res_FinishBattle() { Result = ERROR_CODE.FAIL_INVALID_USER };

            var sres = await _playerService.FinishBattleAsync(
                userId,
                req.BattleSessionID,
                req.OpponentID,
                req.BattleResult,
                req.OpponentDestroyedEntityUIDs,
                req.PlayTime);

            if (sres.errCode != ERROR_CODE.SUCCESS)
                return new() { Result = sres.errCode };

            return sres.ToFinishBattleResponse();
        }

        [HttpPost(nameof(RepairEntities))]
        [Authorize]
        public async Task<Res_RepairEntities> RepairEntities([FromBody] Req_RepairEntities req)
        {
            if (req == null || req.TargetEntityUIDs == null || req.TargetEntityUIDs.Length == 0)
                return new Res_RepairEntities() { Result = ERROR_CODE.FAIL_EMPTY_REQUEST };

            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
                return new Res_RepairEntities() { Result = ERROR_CODE.FAIL_INVALID_USER };

            var sres = await _playerService.RepairEntitiesAsync(userId, req.TargetEntityUIDs, req.ExpectedTotalGold, req.ExpectedTotalWood, req.ExpectedTotalFood);

            if (sres.errCode != ERROR_CODE.SUCCESS)
                return new() { Result = sres.errCode };

            return sres.ToRepairEntitiesResponse(_tableService);
        }
    }
}
