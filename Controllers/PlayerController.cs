using LearningServer01.Repositories;
using Microsoft.AspNetCore.Mvc;
using JNetwork;
using Microsoft.AspNetCore.Identity.Data;

namespace LearningServer01.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerRepository _repository;
        public PlayerController(IPlayerRepository repository)
        {
            _repository = repository;
        }

        [HttpPost(nameof(Register))]
        public async Task<Res_RegisterAccount> Register([FromBody] Req_RegisterAccount req)
        {
            // TEST 
            await Task.Delay(5000);

            var res = new Res_RegisterAccount();

            if (req == null || string.IsNullOrEmpty(req.AccountID) || string.IsNullOrEmpty(req.Password))
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
                res.Result = ERROR_CODE.LOGIN_FAIL_INVALID;
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

            res.Level = user.Level;
            res.Gold = user.Gold;
            res.Wood = user.Wood;
            res.Food = user.Food;
            res.SkillIDs = new[] { user.SkillID01, user.SkillID02, user.SkillID03 };
            res.SpellIDs = new[] { user.SpellID01, user.SpellID02, user.SpellID03 };

            return res;
        }
    }
}
