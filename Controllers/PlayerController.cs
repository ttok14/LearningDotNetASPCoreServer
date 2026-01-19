using LearningServer01.Repositories;
using Microsoft.AspNetCore.Mvc;
using JNetwork;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LearningServer01.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
        public async Task<Res_RegisterAccount> Register([FromBody] Req_RegisterAccount req)
        {
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
                SpellID03 = 24
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
        public async Task<Res_Login> Login([FromBody] Req_Login req)
        {
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

            return res;
        }

        [HttpPost(nameof(CheatAddGold))]
        public async Task<Res_CheatAddGold> CheatAddGold([FromBody] Req_CheatAddGold req)
        {
            var res = new Res_CheatAddGold();

            if (req == null)
            {
                res.Result = ERROR_CODE.FAIL_EMPTY_REQUEST;
                return res;
            }

            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
            {
                res.Result = ERROR_CODE.FAIL_ETC;
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
                res.Result = ERROR_CODE.FAIL_ETC;
                return res;
            }

            res.Result = ERROR_CODE.SUCCESS;
            res.CurrentAmount = (await _repository.GetPlayerAsync(userId)).Gold;

            return res;
        }

        [HttpPost(nameof(CheatAddWood))]
        public async Task<Res_CheatAddWood> CheatAddWood([FromBody] Req_CheatAddWood req)
        {
            var res = new Res_CheatAddWood();

            if (req == null)
            {
                res.Result = ERROR_CODE.FAIL_EMPTY_REQUEST;
                return res;
            }

            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
            {
                res.Result = ERROR_CODE.FAIL_ETC;
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
                res.Result = ERROR_CODE.FAIL_ETC;
                return res;
            }

            res.Result = ERROR_CODE.SUCCESS;
            res.CurrentAmount = (await _repository.GetPlayerAsync(userId)).Wood;

            return res;
        }

        [HttpPost(nameof(CheatAddFood))]
        public async Task<Res_CheatAddFood> CheatAddFood([FromBody] Req_CheatAddFood req)
        {
            var res = new Res_CheatAddFood();

            if (req == null)
            {
                res.Result = ERROR_CODE.FAIL_EMPTY_REQUEST;
                return res;
            }

            var userId = GetUserID();

            if (string.IsNullOrEmpty(userId))
            {
                res.Result = ERROR_CODE.FAIL_ETC;
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
                res.Result = ERROR_CODE.FAIL_ETC;
                return res;
            }

            res.Result = ERROR_CODE.SUCCESS;
            res.CurrentAmount = (await _repository.GetPlayerAsync(userId)).Food;

            return res;
        }

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
    }
}
