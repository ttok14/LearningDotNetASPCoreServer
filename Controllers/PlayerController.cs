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

namespace LearningServer01.Controllers
{
    [ApiController]
    [Route("[controller]")]
    // 
    [Authorize]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerRepository _repository;
        private readonly IConfiguration _configuration;

        string? GetUserID() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public PlayerController(IPlayerRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
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

            // 신규회원 기본 세팅값
            var isSuccess = await _repository.AddPlayerAsync(new PlayerInfo()
            {
                ID = req.AccountID,
                Password = BCrypt.Net.BCrypt.HashPassword(req.Password),
                Level = 1,
                Gold = 10000,
                Wood = 10000,
                Food = 10000,
                SkillID01 = 1,
                SkillID02 = 15,
                SkillID03 = 12,
                SpellID01 = 22,
                SpellID02 = 23,
                SpellID03 = 24,
                Structures = DefaultStructures(req.AccountID)
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
            res.Token = CreateToken(user.ID);
            res.Level = user.Level;
            res.Gold = user.Gold;
            res.Wood = user.Wood;
            res.Food = user.Food;
            res.SkillIDs = new[] { user.SkillID01, user.SkillID02, user.SkillID03 };
            res.SpellIDs = new[] { user.SpellID01, user.SpellID02, user.SpellID03 };
            res.Structures = user.Structures.Select(t => new StructureItem()
            {
                UID = t.UID,
                TableID = t.TableID,
                Level = t.Level,
                PositionX = t.PositionX,
                PositionZ = t.PositionZ,
                RotationY = t.RotationY
            }).ToList();

            return res;
        }

        [HttpPost(nameof(CheatAddGold))]
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

        [HttpPost(nameof(CheatAddWood))]
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

        [HttpPost(nameof(CheatAddFood))]
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

        private List<StructureInfo> DefaultStructures(string userId)
        {
            return new List<StructureInfo>()
            {
                new StructureInfo()
                {
                    TableID = 156, Level = 1, OwnerID = userId,
                    PositionX = 3.3f, PositionZ = 4.6f, RotationY = -90
                },
                new StructureInfo()
                {
                    TableID = 163, Level = 1, OwnerID = userId,
                    PositionX = 22.5f, PositionZ = 4.7f, RotationY = -180
                },
                new StructureInfo()
                {
                    TableID = 157, Level = 1, OwnerID = userId,
                    PositionX = 33.3f, PositionZ = 3.5f, RotationY = -180
                },
                new StructureInfo()
                {
                    TableID = 372, Level = 1, OwnerID = userId,
                    PositionX = 8f, PositionZ = 17f, RotationY = -90
                },
                new StructureInfo()
                {
                    TableID = 144, Level = 1, OwnerID = userId,
                    PositionX = 21.32f, PositionZ = 28.56f, RotationY = -180
                },
                new StructureInfo()
                {
                    TableID = 145, Level = 1, OwnerID = userId,
                    PositionX = 31.04f, PositionZ = 27.6f, RotationY = -180
                },
            };
        }
    }
}
