using GameDB;
using System.Collections;
using System.Collections.Generic;

namespace JNetwork
{
    public class EntityNetData
    {
        public string OwnerID { get; set; }
        public long UID { get; set; }
        public int TableID { get; set; }
        public int Level { get; set; }
        public bool NeedsRepair { get; set; }

        public float PositionX { get; set; }
        public float PositionZ { get; set; }
        public float RotationY { get; set; }

        public StructureSpecificData StructureData { get; set; }
    }

    public class BattleLogNetData
    {
        public long ID { get; set; }
        public string SessionId { get; set; }

        public string AttackerId { get; set; }
        public string AttackerNickname { get; set; }
        public string DefenderId { get; set; }
        public string DefenderNickname { get; set; }

        public S_BattleResult BattleResult { get; set; }
        public S_BattleModeType BattleMode { get; set; }

        public int LootedGold { get; set; }
        public int LootedWood { get; set; }
        public int LootedFood { get; set; }

        public bool IsRevenged { get; set; }

        public long LogTimeUtcTicks { get; set; }
    }

    public class StructureSpecificData
    {
        public List<long> GarrisonedItemUIDs { get; set; } = new List<long>();
        public List<int> GarrisonedEntityTableIDs { get; set; } = new List<int>();

        public long SpawningItemUID { get; set; }
        public int SpawningItemTableID { get; set; }
    }

    public class UserItemNetData
    {
        public long UID { get; set; }
        public int TableID { get; set; }
        public int Quantity { get; set; }
    }

    public class DeploymentSlotNetData
    {
        public int SlotIdx { get; set; }

        public long EquippedItemUID { get; set; }
    }

    public class BattleDeploymentSlotNetData
    {
        public int SlotIdx { get; set; }

        public long EquippedItemUID { get; set; }

        public int TableID { get; set; }
    }
}
