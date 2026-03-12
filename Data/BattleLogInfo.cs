using JNetwork;
using LearningServer01.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearningServer01
{
    public class BattleLogInfo
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public string SessionId { get; set; }

        [Required]
        public string DefenderId { get; set; }
        [Required]
        public string DefenderNickname { get; set; }

        [Required]
        public string AttackerId { get; set; }
        [Required]
        public string AttackerNickname { get; set; }

        public S_BattleResult BattleResult { get; set; }

        public DateTime LogTimeUtc { get; set; }

        public S_BattleModeType ModeType { get; set; }

        public bool IsRevenged { get; set; }

        public int LootedGold { get; set; }
        public int LootedWood { get; set; }
        public int LootedFood { get; set; }

        [ForeignKey(nameof(DefenderId))]
        public PlayerInfo DefenderPlayer { get; set; }

        [ForeignKey(nameof(AttackerId))]
        public PlayerInfo AttackerPlayer { get; set; }
    }
}
