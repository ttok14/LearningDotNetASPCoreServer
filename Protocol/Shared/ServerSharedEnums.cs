namespace JNetwork
{
    public enum S_GarrisonSlotType : byte
    {
        None = 0,

        Garrison = 1,
        Spawn = 2,
    }

    public enum S_BattleModeType : byte
    {
        None = 0,

        Practice,
        Revenge,
        Search
    }

    public enum S_BattleResult : byte
    {
        None = 0,

        Win = 1,
        Lose = 2,
        Draw = 3,
    }
}
