using LearningServer01.MemoryCache;
using LearningServer01.Services.PlayerService;

namespace LearningServer01.BackgroundServices
{
    // BackgroundService를 상속받으면 서버가 켜질 때 자동으로 같이 실행됩니다.
    public class GlobalZombieBattleSessionCleaner : BackgroundService
    {
        private readonly ActiveBattleCache _cache;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<GlobalZombieBattleSessionCleaner> _logger;

        public GlobalZombieBattleSessionCleaner(
            ActiveBattleCache cache,
            IServiceScopeFactory scopeFactory, // DB 컨텍스트(Scoped)를 쓰기 위한 팩토리
            ILogger<GlobalZombieBattleSessionCleaner> logger)
        {
            _cache = cache;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("글로벌 좀비 청소기 작동 시작...");

            // 서버가 꺼질 때(stoppingToken)까지 무한 루프
            while (stoppingToken.IsCancellationRequested == false)
            {
                var expiredSessions = _cache.GetExpiredSessions(TimeSpan.FromMinutes(5));

                if (expiredSessions.Count > 0)
                {
                    // BackgroundService는 Singleton이라서 바로 DB를 못쓰기때문에 
                    // 매번 Scope를 열어서 PlayerService를 꺼내 써야함
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var playerService = scope.ServiceProvider.GetRequiredService<IPlayerService>();

                        foreach (var session in expiredSessions)
                        {
                            try
                            {
                                await playerService.AddBattleLog(
                                    session.SessionId,
                                    session.AttackerId,
                                    session.AttackerNickname,
                                    session.DefenderId,
                                    session.DefenderNickname,
                                    DateTime.UtcNow,
                                    JNetwork.S_BattleResult.Lose,
                                    0, 0, 0,
                                    session.ModeType);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex.Message);
                            }
                            finally
                            {
                                if (_cache.TryRemove(session.AttackerId, out var removed) == false)
                                    _logger.LogError($"좀비 배틀 세션 제거중 이상 현상감지 - 세션의 AttackId 삭제 실패");
                                else if (removed.DefenderId != session.DefenderId)
                                    _logger.LogError($"좀비 배틀 세션 제거중 이상 현상감지 - 삭제된 세션의 DefenderId 가 현 세션에 설정돼있던 DefenderId 랑 다름");
                            }
                        }
                    }
                }

                // 1분 후 실행
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
