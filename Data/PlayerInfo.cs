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

        public int Level { get; set; }

        public int Gold { get; set; }
        public int Wood { get; set; }
        public int Food { get; set; }

        public int SkillID01 { get; set; }
        public int SkillID02 { get; set; }
        public int SkillID03 { get; set; }

        public int SpellID01 { get; set; }
        public int SpellID02 { get; set; }
        public int SpellID03 { get; set; }

        public List<StructureInfo> Structures { get; set; } = new List<StructureInfo>();
    }
}
