using System.Collections.Generic;
using System.Numerics;
using GameDB;

namespace JNetwork
{
    public abstract class WebPacketBase
    {
        public abstract Code ID { get; }
        public abstract string GetURLPath();
    }

    public abstract class WebResponseBase
    {
        public ERROR_CODE Result { get; set; }
    }

    #region ====:: 요청 DTO ::====

    public class Req_CheckVersion : WebPacketBase
    {
        public override Code ID => Code.CheckVersion;
        public override string GetURLPath() => "Player/CheckVersion";

        public string Platform { get; set; }
        public string ClientAppVersion { get; set; }
    }

    public class Req_RegisterAccount : WebPacketBase
    {
        public override Code ID => Code.RegisterAccount;
        public override string GetURLPath() => "Player/Register";

        public string AccountID { get; set; }
        public string Password { get; set; }
    }

    public class Req_Login : WebPacketBase
    {
        public override Code ID => Code.Login;
        public override string GetURLPath() => "Player/Login";

        public string AccountID { get; set; }
        public string Password { get; set; }
    }

    public class Req_EnterNickname : WebPacketBase
    {
        public override Code ID => Code.EnterNickname;
        public override string GetURLPath() => "Player/EnterNickname";

        public string Nickname { get; set; }
    }

    public class Req_SetStatusMessage : WebPacketBase
    {
        public override Code ID => Code.SetStatusMessage;
        public override string GetURLPath() => "Player/SetStatusMessage";

        public string Message { get; set; }
    }

    public class Req_EnterHome : WebPacketBase
    {
        public override Code ID => Code.EnterHome;
        public override string GetURLPath() => "Player/EnterHome";
    }

    public class Req_SearchOpponent : WebPacketBase
    {
        public override Code ID => Code.SearchOpponent;
        public override string GetURLPath() => "Player/SearchOpponent";
    }

    public class Req_CheatAddCurrency : WebPacketBase
    {
        public override Code ID => Code.ChangeSkill;
        public override string GetURLPath() => "Player/CheatAddCurrency";

        public E_CurrencyType CurrencyType { get; set; }
        public int Amount { get; set; }
    }

    public class Req_ChangeSkill : WebPacketBase
    {
        public override Code ID => Code.ChangeSkill;
        public override string GetURLPath() => "Player/ChangeSkill";

        public int[] SkillSet { get; set; }
        public int[] SpellSet { get; set; }
    }

    public class Req_EquipDeploymentSlot : WebPacketBase
    {
        public override Code ID => Code.EquipDeploymentSlot;
        public override string GetURLPath() => "Player/EquipDeploymentSlot";

        public int EquipSlotIdx { get; set; }
        public long EquipItemUid { get; set; }
    }

    public class Req_UnequipDeploymentSlot : WebPacketBase
    {
        public override Code ID => Code.UnequipDeploymentSlot;
        public override string GetURLPath() => "Player/UnequipDeploymentSlot";

        public int SlotIdx { get; set; }
    }

    public class Req_CreateStructure : WebPacketBase
    {
        public override Code ID => Code.CreateStructure;
        public override string GetURLPath() => "Player/CreateStructure";

        public int TableID { get; set; }
        public float PositionX { get; set; }
        public float PositionZ { get; set; }
        public float RotationY { get; set; }
    }

    public class Req_DestroyStructure : WebPacketBase
    {
        public override Code ID => Code.DestroyStructure;
        public override string GetURLPath() => "Player/DestroyStructure";

        public long UID { get; set; }
    }

    public class Req_GarrisonUnit : WebPacketBase
    {
        public override Code ID => Code.GarrisonUnit;
        public override string GetURLPath() => "Player/GarrisonUnit";

        public long OwnerEntityUid { get; set; }
        public int SlotIdx { get; set; }
        public long GarrisonUnitUid { get; set; }
    }

    public class Req_SetSpawnUnit : WebPacketBase
    {
        public override Code ID => Code.SetSpawnUnit;
        public override string GetURLPath() => "Player/SetSpawnUnit";

        public long OwnerEntityUid { get; set; }
        public long SpawnUnitUid { get; set; }
    }

    public class Req_UngarrisonUnit : WebPacketBase
    {
        public override Code ID => Code.UngarrisonUnit;
        public override string GetURLPath() => "Player/UngarrisonUnit";

        public long OwnerEntityUid { get; set; }
        public long UngarrisonUnitUid { get; set; }
    }

    public class Req_BuyItem : WebPacketBase
    {
        public override Code ID => Code.BuyItem;
        public override string GetURLPath() => "Player/BuyItem";

        public int ItemTid { get; set; }
    }

    public class Req_EquipHero : WebPacketBase
    {
        public override Code ID => Code.EquipHero;
        public override string GetURLPath() => "Player/EquipHero";

        public long HeroItemUid { get; set; }
    }

    public class Req_UnequipHero : WebPacketBase
    {
        public override Code ID => Code.UnequipHero;
        public override string GetURLPath() => "Player/UnequipHero";
    }

    public class Req_MoveEntity : WebPacketBase
    {
        public override Code ID => Code.MoveEntity;
        public override string GetURLPath() => "Player/MoveEntity";

        public long MoveEntityUid { get; set; }
        public float PositionX { get; set; }
        public float PositionZ { get; set; }
        public float RotationY { get; set; }
    }

    public class Req_StartBattle : WebPacketBase
    {
        public override Code ID => Code.StartBattle;
        public override string GetURLPath() => "Player/StartBattle";

        public string OpponentPlayerId { get; set; }
        public S_BattleModeType ModeType { get; set; }

        // 복수 전투 시작 시 사용된 배틀 로그 UID (기본값 0 = 해당 없음)
        public long TargetBattleLogUid { get; set; } = 0;
    }

    public class Req_LoadRevenge : WebPacketBase
    {
        public override Code ID => Code.LoadRevenge;
        public override string GetURLPath() => "Player/LoadRevenge";

        public long BattleLogUid { get; set; }
        public string OpponentId { get; set; }
    }

    public class Req_FinishBattle : WebPacketBase
    {
        public override Code ID => Code.FinishBattle;
        public override string GetURLPath() => "Player/FinishBattle";

        public string BattleSessionID { get; set; }
        public string OpponentID { get; set; }
        public S_BattleResult BattleResult { get; set; }
        public long[] OpponentDestroyedEntityUIDs { get; set; }
        public float PlayTime { get; set; }
    }

    public class Req_RepairEntities : WebPacketBase
    {
        public override Code ID => Code.RepairEntities;
        public override string GetURLPath() => "Player/RepairEntities";

        public long[] TargetEntityUIDs { get; set; }
        public long ExpectedTotalGold { get; set; }
        public long ExpectedTotalWood { get; set; }
        public long ExpectedTotalFood { get; set; }
    }

    #endregion

    #region ====:: 응답 DTO ::====

    public class Res_CheckVersion : WebResponseBase
    {
        public string Message { get; set; }

        public string LatestAppVersion { get; set; }
        public string CdnBaseUrl { get; set; }
        public string RedirectStoreUrl { get; set; }

        public string TableMetadataHash { get; set; }
    }

    public class Res_RegisterAccount : WebResponseBase
    {
    }

    public class Res_Login : WebResponseBase
    {
        public string Token { get; set; }
        public string OwnerId { get; set; }
        public string Nickname { get; set; }
        public string StatusMsg { get; set; }

        public int Bounty { get; set; }
        public long EquippedHeroItemUID { get; set; }

        public int StrengthStat { get; set; }

        public int Level { get; set; }

        public int Gold { get; set; }
        public int Wood { get; set; }
        public int Food { get; set; }

        public int[] SkillIDs { get; set; }
        public int[] SpellIDs { get; set; }

        public List<BattleLogNetData> BattleLogs { get; set; }
        public List<EntityNetData> Entities { get; set; }
        public List<UserItemNetData> Items { get; set; }
        public List<DeploymentSlotNetData> DeploymentSlots { get; set; }
    }

    public class Res_EnterNickname : WebResponseBase
    {

    }

    public class Res_SetStatusMessage : WebResponseBase
    {
    }

    public class Res_EnterHome : WebResponseBase
    {
        public string MapName { get; set; }
        public List<EntityNetData> Entities { get; set; }
    }

    public class Res_SearchOpponent : WebResponseBase
    {
        public string ID { get; set; }
        public string EnemyPlayerNickname { get; set; }
        public string StatusMsg { get; set; }

        public int OpponentLevel { get; set; }
        public int Bounty { get; set; }

        public int StrengthStat { get; set; }

        public string MapName { get; set; }

        public int RemainedGold { get; set; }

        public List<EntityNetData> Entities { get; set; }
        public BattleDeploymentSlotNetData HeroSlot { get; set; }
        public List<BattleDeploymentSlotNetData> DeploymentSlots { get; set; }
    }

    public class Res_CheatAddCurrency : WebResponseBase
    {
        public int CurrentAmount { get; set; }
    }

    public class Res_ChangeSkill : WebResponseBase
    {
    }

    public class Res_EquipDeploymentSlot : WebResponseBase
    {
    }

    public class Res_UnequipDeploymentSlot : WebResponseBase
    {
    }

    public class Res_CreateStructure : WebResponseBase
    {
        public long UID { get; set; }

        public int RemainedGold { get; set; }
        public int RemainedWood { get; set; }
        public int RemainedFood { get; set; }

        public StructureSpecificData StructureData { get; set; }
    }

    public class Res_DestroyStructure : WebResponseBase
    {
    }

    public class Res_GarrisonUnit : WebResponseBase
    {
    }

    public class Res_SetSpawnUnit : WebResponseBase
    {
    }

    public class Res_UngarrisonUnit : WebResponseBase
    {
        public S_GarrisonSlotType OperatedGarrisonType { get; set; }
    }

    public class Res_BuyItem : WebResponseBase
    {
        public long UID { get; set; }
        public int SlotIdx { get; set; }

        public int RemainedGold { get; set; }
        public int RemainedWood { get; set; }
        public int RemainedFood { get; set; }
    }

    public class Res_MoveEntity : WebResponseBase
    {
    }

    public class Res_EquipHero : WebResponseBase
    {
    }

    public class Res_UnequipHero : WebResponseBase
    {
    }

    public class Res_StartBattle : WebResponseBase
    {
        public string SessionId { get; set; }
    }

    public class Res_LoadRevenge : WebResponseBase
    {
        public string ID { get; set; }
        public string EnemyPlayerNickname { get; set; }
        public string StatusMsg { get; set; }

        public int OpponentLevel { get; set; }
        public int Bounty { get; set; }
        public int StrengthStat { get; set; }
        public string MapName { get; set; }

        public List<EntityNetData> Entities { get; set; }
        public BattleDeploymentSlotNetData HeroSlot { get; set; }
        public List<BattleDeploymentSlotNetData> DeploymentSlots { get; set; }
    }

    public class Res_FinishBattle : WebResponseBase
    {
        public BattleLogNetData AddedBattleLog { get; set; }

        public long[] RemovedBattleLogs { get; set; }

        public long RewardGold { get; set; }
        public long TotalGold { get; set; }
        public long RewardWood { get; set; }
        public long TotalWood { get; set; }
        public long RewardFood { get; set; }
        public long TotalFood { get; set; }
        public int AddedBounty { get; set; }
        public int TotalBounty { get; set; }
    }

    public class Res_RepairEntities : WebResponseBase
    {
        public long RemainGold { get; set; }
        public long RemainWood { get; set; }
        public long RemainFood { get; set; }
        public long[] RepairedEntityUIDs { get; set; }
    }

    #endregion
}
