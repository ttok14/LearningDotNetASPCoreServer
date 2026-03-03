using LearningServer01.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearningServer01
{
    public class PlayerInfo
    {
        // 고유 식별자(PK ,Primary Key) 설정 
        [Key]
        public string ID { get; set; }
        public string Password { get; set; }
        public string StatusMsg { get; set; }

        public string Nickname { get; set; }

        public int Level { get; set; }

        public int Bounty { get; set; }

        public long EquippedHeroItemUID { get; set; }

        public int StrengthStat { get; set; }

        public int Gold { get; set; }
        public int Wood { get; set; }
        public int Food { get; set; }

        public int SkillID01 { get; set; }
        public int SkillID02 { get; set; }
        public int SkillID03 { get; set; }

        public int SpellID01 { get; set; }
        public int SpellID02 { get; set; }
        public int SpellID03 { get; set; }

        public List<UserItem> InventoryItems { get; set; } = new List<UserItem>();

        public List<DeploymentSlot> DeploymentSlots { get; set; } = new List<DeploymentSlot>();

        public List<EntityItemInfo> PlacedEntities { get; set; } = new List<EntityItemInfo>();
    }
}
