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

        // 건물 : 가드배치정보 등, 엔티티 타입에 따라 달라지는
        // 가변 데이터를 Json String 형태로 관리
        // 없으면 Null 허용
        public string? SpecificDataJson { get; set; }

        [ForeignKey(nameof(OwnerID))]
        public PlayerInfo Owner { get; set; }

    }
}
