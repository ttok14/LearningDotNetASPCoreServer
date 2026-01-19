
namespace JNetwork
{
    // 패킷 아이디 
    public enum Code : ushort
    {
        RegisterAccount = 110,
        Login = 120,

        CheatAddGold = 201,
        CheatAddWood = 202,
        CheatAddFood = 203,
    }

    public enum ERROR_CODE : ushort
    {
        SUCCESS = 0,

        FAIL_INVALID_TOKEN = 5,

        FAIL_ETC = 10,

        FAIL_EMPTY_REQUEST = 50,

        #region ====:: 계정 관련 (1000~1100)::====
        REGISTER_FAIL_INVALID = 1015,
        REGISTER_FAIL_DUPLICATE = 1020,
        REGISTER_FAIL_UNKNOWN = 1025,

        LOGIN_FAIL_USER_NOT_EXIST = 1035,
        LOGIN_FAIL_PW_WRONG = 1040,
        #endregion
    }
}
