using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearningServer01.Data
{
    public class DeploymentSlot
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SlotUID { get; set; }

        [Required]
        public string OwnerID { get; set; }

        public int SlotIdx { get; set; }

        // 해당 Item 이 만약 사라진다면 슬롯이 비어져야하기에 Nullable 선언 
        public long? EquippedItemUID { get; set; }

        [ForeignKey(nameof(EquippedItemUID))]
        public UserItem EquippedItem { get; set; }

        [ForeignKey(nameof(OwnerID))]
        public PlayerInfo Owner { get; set; }

        //--------------//

        public void EquipItem(UserItem item)
        {
            if (item == null)
            {
                UnequipItem();
                return;
            }

            EquippedItem = item;
            EquippedItemUID = item.UID;
        }

        public void UnequipItem()
        {
            if (EquippedItemUID.HasValue == false)
                return;

            EquippedItemUID = null;
            EquippedItem = null;
        }
    }
}
