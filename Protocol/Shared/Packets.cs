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

    #endregion

    #region ====:: 응답 DTO ::====

    public class Res_RegisterAccount : WebResponseBase
    {
    }

    public class Res_Login : WebResponseBase
    {
        public int Level { get; set; }

        public int Gold { get; set; }
        public int Wood { get; set; }
        public int Food { get; set; }

        public int[] SkillIDs { get; set; }
        public int[] SpellIDs { get; set; }
    }

    #endregion
}
