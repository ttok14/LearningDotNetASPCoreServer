using LearningServer01.Repositories;
using Microsoft.AspNetCore.Mvc;
using JNetwork;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using LearningServer01.Data;
using GameDB;

namespace LearningServer01.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly ITableService _tableService;

        string? GetUserID() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public PlayerController(IPlayerRepository repository, IConfiguration configuration, ITableService tableService)
        {
            _repository = repository;
            _configuration = configuration;
            _tableService = tableService;
        }

        [HttpPost(nameof(CheckVersion))]
        [AllowAnonymous]
        public async Task<Res_CheckVersion> CheckVersion([FromBody] Req_CheckVersion req)
        {
            Console.WriteLine(nameof(CheckVersion));

            var res = new Res_CheckVersion();

            if (req == null)
            {
                res.Result = ERROR_CODE.FAIL_EMPTY_REQUEST;
                return res;
            }

            var validAppVersion = _configuration.GetValue<string>("GameSettings:AppVersion");

            if (req.ClientAppVersion != validAppVersion)
            {
                res.Result = ERROR_CODE.FAIL_INVALID_APP_VERSION;
                res.Message = "앱 재다운로드 필요";

                // TODO : 나중에는 store url 넣어줘야할듯 
                // if(req.Platform)
                res.RedirectStoreUrl = "store_url";
                return res;
            }

            bool isMaintenance = _configuration.GetValue<bool>("GameSettings:IsMaintenance");
            if (isMaintenance)
            {
                res.Result = ERROR_CODE.FAIL_MAINTENANCE;
                // message 도 appsettings 에 넣어야할지는 고민 
                res.Message = $"서버 점검 (n시 종료)";
                return res;
            }

            res.Result = ERROR_CODE.SUCCESS;

            res.CdnBaseUrl = _configuration.GetValue<string>("GameSettings:CdnBaseUrl");
            res.LatestAppVersion = validAppVersion;
            res.TableMetadataHash = _tableService.Metadata.TotalHash;

            return res;
        }

        [HttpPost(nameof(Register))]
        [AllowAnonymous]
        public async Task<Res_RegisterAccount> Register([FromBody] Req_RegisterAccount req)
        {
            Console.WriteLine(nameof(Register));

            var res = new Res_RegisterAccount();

            if (req == null)
            {
                res.Result = ERROR_CODE.FAIL_EMPTY_REQUEST;
                return res;
            }

            if (string.IsNullOrEmpty(req.AccountID) || string.IsNullOrEmpty(req.Password))
            {
                res.Result = ERROR_CODE.REGISTER_FAIL_INVALID;
                return res;
            }

            var duplicate = await _repository.GetPlayerAsync(req.AccountID);
            if (duplicate != null)
            {
                res.Result = ERROR_CODE.REGISTER_FAIL_DUPLICATE;
                return res;
            }

            var defaultEntities = DefaultEntities(req.AccountID);
            var defaultItems = DefaultItems(req.AccountID);
            var defaultDeploymentSlots = DefaultDeploymentSlots(req.AccountID, defaultItems);

            // 신규회원 기본 세팅값
            var isSuccess = await _repository.AddPlayerAsync(new PlayerInfo()
            {
                ID = req.AccountID,
                Password = BCrypt.Net.BCrypt.HashPassword(req.Password),
                Level = 1,
                Nickname = $"TestNick_{req.AccountID}",
                StatusMsg = "제발 쳐들어오지 마세요 ㅜㅜ",
                Bounty = 1234543,
                EquippedHeroItemUID = 1,
                StrengthStat = 959595,
                Gold = 10000,
                Wood = 10000,
                Food = 10000,
                SkillID01 = 1,
                SkillID02 = 15,
                SkillID03 = 12,
                SpellID01 = 22,
                SpellID02 = 23,
                SpellID03 = 24,
                PlacedEntities = defaultEntities,
                InventoryItems = defaultItems,
                DeploymentSlots = defaultDeploymentSlots
            });

            if (isSuccess == false)
            {
                res.Result = ERROR_CODE.REGISTER_FAIL_UNKNOWN;
                return res;
            }

            res.Result = ERROR_CODE.SUCCESS;

            return res;
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
            Console.WriteLine(nameof(Login));

            var res = new Res_Login();

            if (req == null)
            {
                res.Result = ERROR_CODE.FAIL_EMPTY_REQUEST;
                return res;
            }

            var user = await _repository.GetPlayerAsync(req.AccountID);

            if (user == null)
            {
                res.Result = ERROR_CODE.LOGIN_FAIL_USER_NOT_EXIST;
                return res;
            }

            bool isValid = BCrypt.Net.BCrypt.Verify(req.Password, user.Password);

            if (isValid == false)
            {
                res.Result = ERROR_CODE.LOGIN_FAIL_PW_WRONG;
                return res;
            }

            res.Result = ERROR_CODE.SUCCESS;
            res.OwnerId = req.AccountID;
            res.Token = CreateToken(user.ID);
            res.Nickname = user.Nickname;
            res.StatusMsg = user.StatusMsg;
            res.Bounty = user.Bounty;
            res.EquippedHeroItemUID = user.EquippedHeroItemUID;
            res.StrengthStat = user.StrengthStat;
            res.Level = user.Level;
            res.Gold = user.Gold;
            res.Wood = user.Wood;
            res.Food = user.Food;
            res.SkillIDs = new[] { user.SkillID01, user.SkillID02, user.SkillID03 };
            res.SpellIDs = new[] { user.SpellID01, user.SpellID02, user.SpellID03 };
            res.Entities = user.PlacedEntities.Select(t => new EntityNetData()
            {
                OwnerID = t.OwnerID,
                UID = t.UID,
                TableID = t.TableID,
                Level = t.Level,
                PositionX = t.PositionX,
                PositionZ = t.PositionZ,
                RotationY = t.RotationY
            }).ToList();
            res.Items = user.InventoryItems.Select(t => new UserItemNetData()
            {
                UID = t.UID,
                TableID = t.TableID,
                Quantity = t.Quantity
            }).ToList();

            res.DeploymentSlots = user.DeploymentSlots.Select(t => new DeploymentSlotNetData()
            {
                SlotIdx = t.SlotIdx,
                EquippedItemUID = t.EquippedItemUID ?? 0
            }).ToList();

            return res;
        }

#if DEBUG
        [HttpPost(nameof(CheatAddGold))]
        [Authorize]
        public async Task<Res_CheatAddGold> CheatAddGold([FromBody] Req_CheatAddGold req)
        {
            Console.WriteLine(nameof(CheatAddGold));

            var res = new Res_CheatAddGold();

            if (req == null)
            {
                res.Result = ERROR_CODE.FAIL_EMPTY_REQUEST;
                return res;
            }

            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
            {
                res.Result = ERROR_CODE.FAIL_UNKNOWN;
                return res;
            }

            if (req.Amount == 0)
            {
                res.Result = ERROR_CODE.SUCCESS;
                var player = await _repository.GetPlayerAsync(userId);
                res.CurrentAmount = player.Gold;
                return res;
            }

            if (await _repository.AddGold(userId, req.Amount) == false)
            {
                res.Result = ERROR_CODE.FAIL_UNKNOWN;
                return res;
            }

            res.Result = ERROR_CODE.SUCCESS;
            res.CurrentAmount = (await _repository.GetPlayerAsync(userId)).Gold;

            return res;
        }
#endif

#if DEBUG
        [HttpPost(nameof(CheatAddWood))]
        [Authorize]
        public async Task<Res_CheatAddWood> CheatAddWood([FromBody] Req_CheatAddWood req)
        {
            Console.WriteLine(nameof(CheatAddWood));

            var res = new Res_CheatAddWood();

            if (req == null)
            {
                res.Result = ERROR_CODE.FAIL_EMPTY_REQUEST;
                return res;
            }

            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
            {
                res.Result = ERROR_CODE.FAIL_UNKNOWN;
                return res;
            }

            if (req.Amount == 0)
            {
                res.Result = ERROR_CODE.SUCCESS;
                var player = await _repository.GetPlayerAsync(userId);
                res.CurrentAmount = player.Wood;
                return res;
            }

            if (await _repository.AddWood(userId, req.Amount) == false)
            {
                res.Result = ERROR_CODE.FAIL_UNKNOWN;
                return res;
            }

            res.Result = ERROR_CODE.SUCCESS;
            res.CurrentAmount = (await _repository.GetPlayerAsync(userId)).Wood;

            return res;
        }
#endif

#if DEBUG
        [HttpPost(nameof(CheatAddFood))]
        [Authorize]
        public async Task<Res_CheatAddFood> CheatAddFood([FromBody] Req_CheatAddFood req)
        {
            Console.WriteLine(nameof(CheatAddFood));

            var res = new Res_CheatAddFood();

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

            if (req.Amount == 0)
            {
                res.Result = ERROR_CODE.SUCCESS;
                var player = await _repository.GetPlayerAsync(userId);
                res.CurrentAmount = player.Food;
                return res;
            }

            if (await _repository.AddFood(userId, req.Amount) == false)
            {
                res.Result = ERROR_CODE.FAIL_UNKNOWN;
                return res;
            }

            res.Result = ERROR_CODE.SUCCESS;
            res.CurrentAmount = (await _repository.GetPlayerAsync(userId)).Food;

            return res;
        }
#endif

        [HttpPost(nameof(ChangeSkill))]
        [Authorize]
        public async Task<Res_ChangeSkill> ChangeSkill(Req_ChangeSkill req)
        {
            var res = new Res_ChangeSkill();

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

            bool isSuccess = await _repository.ChangeSkill(userId, req.SkillSet, req.SpellSet);

            if (isSuccess == false)
            {
                res.Result = ERROR_CODE.FAIL_UNKNOWN;
                return res;
            }

            res.Result = ERROR_CODE.SUCCESS;

            return res;
        }

        [HttpPost(nameof(CreateStructure))]
        [Authorize]
        public async Task<Res_CreateStructure> CreateStructure(Req_CreateStructure req)
        {
            Console.WriteLine(nameof(CreateStructure));

            var res = new Res_CreateStructure();

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

            var result = await _repository.CreateStructure(userId, req.TableID, req.PositionX, req.PositionZ, req.RotationY);
            if (result.uid < 0)
            {
                if (result.uid == -1)
                    res.Result = ERROR_CODE.CREATE_STRUCTURE_FAIL_01;
                else if (result.uid == -2)
                    res.Result = ERROR_CODE.CREATE_STRUCTURE_FAIL_02;
                else res.Result = ERROR_CODE.FAIL_UNKNOWN;

                return res;
            }

            res.Result = ERROR_CODE.SUCCESS;
            res.RemainedGold = result.remainedGold;
            res.RemainedWood = result.remainedWood;
            res.RemainedFood = result.remainedFood;
            res.UID = result.uid;

            return res;
        }

        [HttpPost(nameof(DestroyStructure))]
        [Authorize]
        public async Task<Res_DestroyStructure> DestroyStructure(Req_DestroyStructure req)
        {
            Console.WriteLine(nameof(DestroyStructure));

            var res = new Res_DestroyStructure();

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

            bool isSuccess = await _repository.DestroyStructure(userId, req.UID);
            if (isSuccess == false)
            {
                res.Result = ERROR_CODE.FAIL_UNKNOWN;
                return res;
            }

            res.Result = ERROR_CODE.SUCCESS;

            return res;
        }

        //----------------------------------------------------------------------------//

        private string CreateToken(string userId)
        {
            // 토큰에 담을 정보 (Claim)
            // '누구인지' 에 대한 정보만 담아야함. 언제든지 이거는 중간에 볼수있음
            // (비번넣으면안됨)
            var claims = new List<Claim>()
            {
                /// jwt 페이로드에 포함됨 참고.
                /// 참고로 <see cref="ClaimTypes.NameIdentifier"/> 는 "nameid" 로 치환돼 들어감
                new Claim(ClaimTypes.NameIdentifier, userId)
                // 필요하다면 new Claim(ClaimTypes.Role, "Admin") 이런것도 참고 ㄱㄱ 
            };

            // 비밀키 가져오기
            var keyStr = _configuration.GetSection("JwtSettings:SecretKey").Value;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));

            // 서명자격증명
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            /// 토큰의 구성요소들 의미 참고 ㄱㄱ
            ///     https://datatracker.ietf.org/doc/html/rfc7519#page-9 
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration.GetSection("JwtSettings:Issuer").Value,
                Audience = _configuration.GetSection("JwtSettings:Audience").Value,
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private List<EntityItemInfo> DefaultEntities(string userId)
        {
            return new List<EntityItemInfo>()
            {
                new EntityItemInfo()
                {
                    TableID = 156, Level = 1, OwnerID = userId,
                    PositionX = 33.3f, PositionZ = 34.6f, RotationY = -90
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

        private List<UserItem> DefaultItems(string accountID)
        {
            return new List<UserItem>()
            {
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
    }
}
