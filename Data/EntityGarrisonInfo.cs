
using LearningServer01.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GameDB;
using JNetwork;

namespace LearningServer01
{
    public class EntityGarrisonInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UID { get; set; }

        [Required]
        public int SlotIdx { get; set; }

        [Required]
        public long OwnerEntityUID { get; set; }

        [Required]
        public S_GarrisonSlotType Type { get; set; }

        [Required]
        public long EquippedItemUID { get; set; }

        [ForeignKey(nameof(OwnerEntityUID))]
        public EntityItemInfo OwnerStructure { get; set; }

        //--------------------//

        public void Set(long ownerEntityUid, S_GarrisonSlotType type, long equipItemUid)
        {
            OwnerEntityUID = ownerEntityUid;
            Type = type;
            EquippedItemUID = equipItemUid;
        }
    }
}
