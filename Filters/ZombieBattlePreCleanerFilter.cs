using JNetwork;
using LearningServer01;
using LearningServer01.MemoryCache;
using LearningServer01.Services.PlayerService;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Security.Claims;

namespace LearningServer01.Filters
{
    public class ZombieBattlePreCleanerFilter : IAsyncActionFilter
    {
        readonly ILogger<ZombieBattlePreCleanerFilter> _logger;
        readonly ActiveBattleCache _activeBattleCache;
        readonly IPlayerService _playerService;

        public ZombieBattlePreCleanerFilter(
            ILogger<ZombieBattlePreCleanerFilter> logger,
            ActiveBattleCache activeBattleCache,
            IPlayerService playerService)
        {
            _logger = logger;
            _activeBattleCache = activeBattleCache;
            _playerService = playerService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string? callerUserId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (callerUserId == null)
            {
                await next();
                return;
            }

            await _playerService.CleanZombieBattleSessions(callerUserId);
            await next();
        }
    }
}
