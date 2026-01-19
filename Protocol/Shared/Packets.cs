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

    public class Req_RegisterAccount : WebPacketBase
    {
        public override Code ID => Code.RegisterAccount;
        public override string GetURLPath()
        {
            return "Player/Register";
        }

        public string AccountID { get; set; }
        public string Password { get; set; }
    }

    public class Req_Login : WebPacketBase
    {
        public override Code ID => Code.Login;
        public override string GetURLPath()
        {
            return "Player/Login";
        }

        public string AccountID { get; set; }
        public string Password { get; set; }
    }

    public class Req_CheatAddGold : WebPacketBase
    {
        public override Code ID => Code.CheatAddGold;
        public override string GetURLPath()
        {
            return "Player/CheatAddGold";
        }

        public int Amount { get; set; }
    }

    public class Req_CheatAddWood : WebPacketBase
    {
        public override Code ID => Code.CheatAddWood;
        public override string GetURLPath()
        {
            return "Player/CheatAddWood";
        }

        public int Amount { get; set; }
    }

    public class Req_CheatAddFood : WebPacketBase
    {
        public override Code ID => Code.CheatAddFood;
        public override string GetURLPath()
        {
            return "Player/CheatAddFood";
        }

        public int Amount { get; set; }
    }

    #endregion

    #region ====:: 응답 DTO ::====

    public class Res_RegisterAccount : WebResponseBase
    {
    }

    public class Res_Login : WebResponseBase
    {
        public string Token { get; set; }

        public int Level { get; set; }

        public int Gold { get; set; }
        public int Wood { get; set; }
        public int Food { get; set; }

        public int[] SkillIDs { get; set; }
        public int[] SpellIDs { get; set; }
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

    #endregion
}
