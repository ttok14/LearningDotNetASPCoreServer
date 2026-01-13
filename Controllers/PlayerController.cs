using LearningServer01.Repositories;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("{userId}")]
        public ActionResult<PlayerInfo> Get(string userId)
        {
            var info = _repository.GetPlayer(userId);

            if (info == null)
                return NotFound($"{userId}님은 존재하지 않습니다.");

            return Ok(info);
        }

        [HttpPost]
        public ActionResult<bool> CreatePlayer([FromBody] PlayerInfo info)
        {
            if (info == null)
                return BadRequest();

            if (_repository.AddPlayer(info) == false)
                return Conflict("이미 존재하는 닉네임입니다.");

            return Ok();
        }
    }
}
