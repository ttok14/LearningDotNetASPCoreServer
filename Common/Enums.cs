using System;

namespace LearningServer01
{
    [Flags]
    public enum E_PlayerInclude
    {
        None = 0,
        PlacedEntities = 1 << 0, // 하위의 Garrisons 도 포함됨
        InventoryItems = 1 << 1,
        DeploymentSlots = 1 << 2,
        BattleLogs = 1 << 3,

        Full = PlacedEntities | InventoryItems | DeploymentSlots | BattleLogs
    }

    //public enum PlayerState
    //{
    //    None = 0,

    //    Home,
    //    Matching,
    //    InBattle,
    //    FinishedBattle
    //}


    //public enum ValidationRes_Entity
    //{
    //    None = 0,

    //    Success,

    //    InvalidTableService,
    //    EntityNotExist,
    //    InvalidEntityStructure,
    //}

    //public enum ValidationContent
    //{
    //    None = 0,

    //    EntityIsStructure,
    //}
}
