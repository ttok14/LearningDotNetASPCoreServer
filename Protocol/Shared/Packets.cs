using System.Collections.Generic;

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

    public class Req_CheatAddGold : WebPacketBase
    {
        public override Code ID => Code.CheatAddGold;
        public override string GetURLPath() => "Player/CheatAddGold";

        public int Amount { get; set; }
    }

    public class Req_CheatAddWood : WebPacketBase
    {
        public override Code ID => Code.CheatAddWood;
        public override string GetURLPath() => "Player/CheatAddWood";

        public int Amount { get; set; }
    }

    public class Req_CheatAddFood : WebPacketBase
    {
        public override Code ID => Code.CheatAddFood;
        public override string GetURLPath() => "Player/CheatAddFood";

        public int Amount { get; set; }
    }

    public class Req_ChangeSkill : WebPacketBase
    {
        public override Code ID => Code.ChangeSkill;
        public override string GetURLPath() => "Player/ChangeSkill";

        public int[] SkillSet { get; set; }
        public int[] SpellSet { get; set; }
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

        public List<EntityNetData> Entities { get; set; }
        public List<UserItemNetData> Items { get; set; }
        public List<DeploymentSlotNetData> DeploymentSlots { get; set; }
    }

    public class Res_CheatAddGold : WebResponseBase
    {
        public int CurrentAmount { get; set; }
    }

    public class Res_CheatAddWood : WebResponseBase
    {
        public int CurrentAmount { get; set; }
    }

    public class Res_CheatAddFood : WebResponseBase
    {
        public int CurrentAmount { get; set; }
    }

    public class Res_ChangeSkill : WebResponseBase
    {

    }

    public class Res_CreateStructure : WebResponseBase
    {
        public long UID { get; set; }

        public int RemainedGold { get; set; }
        public int RemainedWood { get; set; }
        public int RemainedFood { get; set; }
    }

    public class Res_DestroyStructure : WebResponseBase
    {
    }

    #endregion
}
