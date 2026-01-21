using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearningServer01.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ToolController : ControllerBase
    {
        AppDbContext _context;
        public ToolController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost(nameof(ClearAllTableData))]
        public async Task<ActionResult<bool>> ClearAllTableData()
        {
            try
            {
                await _context.Structures.ExecuteDeleteAsync();
                await _context.Players.ExecuteDeleteAsync();

                return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Error : {ex.Message}");
            }
        }
    }
}
