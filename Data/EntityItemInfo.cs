using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearningServer01.Data
{
    public class EntityItemInfo
    {
        // 고유 식별자(PK ,Primary Key) 설정 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UID { get; set; }

        [Required]
        public string OwnerID { get; set; }

        public int TableID { get; set; }

        public int Level { get; set; }

        public float PositionX { get; set; }
        public float PositionZ { get; set; }
        public float RotationY { get; set; }

        public bool NeedsRepair { get; set; }

        public List<EntityGarrisonInfo> Garrisons { get; set; } = new List<EntityGarrisonInfo>();

        [ForeignKey(nameof(OwnerID))]
        public PlayerInfo Owner { get; set; }

    }
}
