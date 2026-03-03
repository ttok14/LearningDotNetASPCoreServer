
using LearningServer01.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GameDB;

namespace LearningServer01
{
    public class UserItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UID { get; set; }

        [Required]
        public string OwnerID { get; set; }

        public int TableID { get; set; }
        public int Level { get; set; }
        public int Quantity { get; set; }

        [ForeignKey(nameof(OwnerID))]
        public PlayerInfo Owner { get; set; }
    }
}
