using JNetwork;
using System.Collections.Concurrent;
using System.Globalization;

namespace LearningServer01.MemoryCache
{
    public class ActiveBattleCache
    {
        public readonly struct BattleSession
        {
            public readonly string SessionId;
            public readonly string AttackerId;
            public readonly string AttackerNickname;
            public readonly string DefenderId;
            public readonly string DefenderNickname;
            public readonly DateTime StartTimeUtc;
            public readonly S_BattleModeType ModeType;

            public BattleSession(string sessionId, string attackerId, string attackerNickname, string defenderId, string defenderNickname, DateTime startTimeUtc, S_BattleModeType modeType)
            {
                SessionId = sessionId;
                AttackerId = attackerId;
                AttackerNickname = attackerNickname;
                DefenderId = defenderId;
                DefenderNickname = defenderNickname;
                StartTimeUtc = startTimeUtc;
                ModeType = modeType;
            }
        }

        // 두 Dictionary 는 항상 ThreadSafe 하게 동시에 수정이 이루어져야만 하며
        // 이미 들어있는 데이터는 무결성함을 보장
        readonly ConcurrentDictionary<string, BattleSession> _battleSessionsByAttackerId = new ConcurrentDictionary<string, BattleSession>();
        // DefenderId / AttackerId
        readonly ConcurrentDictionary<string, string> _defendersUnderAttack = new ConcurrentDictionary<string, string>();

        readonly object _lockObj = new object();

        readonly ILogger<ActiveBattleCache> _logger;

        public ActiveBattleCache(ILogger<ActiveBattleCache> logger)
        {
            _logger = logger;
        }

        public bool IsAttacking(string id) => _battleSessionsByAttackerId.ContainsKey(id);
        public bool IsUnderAttack(string id) => _defendersUnderAttack.ContainsKey(id);
        public bool TryGetSession(string attackerId, out BattleSession res) => _battleSessionsByAttackerId.TryGetValue(attackerId, out res);
        public bool TryPopTargetSession(string attackerId, string targetSessionId, out BattleSession resValidSession)
        {
            lock (_lockObj)
            {
                resValidSession = default;

                if (_battleSessionsByAttackerId.TryGetValue(attackerId, out var session) == false)
                    return false;

                if (session.SessionId == targetSessionId)
                {
                    TryRemove(attackerId, out resValidSession);
                    return true;
                }

                return false;
            }
        }

        public List<BattleSession> GetExpiredSessions(TimeSpan timeoutThreshold)
        {
            List<BattleSession> expiredSessions = new List<BattleSession>();
            var now = DateTime.UtcNow;

            foreach (var kvp in _battleSessionsByAttackerId)
            {
                if (now - kvp.Value.StartTimeUtc > timeoutThreshold)
                    expiredSessions.Add(kvp.Value);
            }

            return expiredSessions;
        }

        public ERROR_CODE TryRegister(string sessionId, string attackerId, string attackerNickname, string defenderId, string defenderNickname, S_BattleModeType modeType)
        {
            lock (_lockObj)
            {
                if (_battleSessionsByAttackerId.ContainsKey(attackerId) || _defendersUnderAttack.ContainsKey(defenderId))
                    return ERROR_CODE.ZOMBIE_ACTIVE_BATTLE_SESSION_EXIST;

                _battleSessionsByAttackerId[attackerId] = new BattleSession(sessionId, attackerId, attackerNickname, defenderId, defenderNickname, DateTime.UtcNow, modeType);

                _defendersUnderAttack[defenderId] = defenderId;

                return ERROR_CODE.SUCCESS;
            }
        }

        public bool TryRemove(string attackerId, out BattleSession removedSession)
        {
            lock (_lockObj)
            {
                bool attackerRemoved = _battleSessionsByAttackerId.TryRemove(attackerId, out removedSession);
                if (attackerRemoved == false)
                    _logger.LogError($"AttackerID 제거 실패 | AttackerId : {attackerId}");

                bool defenderRemoved = false;

                if (attackerRemoved)
                {
                    defenderRemoved = _defendersUnderAttack.TryRemove(removedSession.DefenderId, out _);
                    if (defenderRemoved == false)
                        _logger.LogError($"DefenderID 제거 실패 | AttackerId : {attackerId}, DefenderId : {removedSession.DefenderId}");
                }

                return attackerRemoved && defenderRemoved;
            }
        }
    }
}
