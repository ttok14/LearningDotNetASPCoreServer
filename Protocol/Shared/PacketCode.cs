
namespace JNetwork
{
    // 패킷 아이디 
    public enum Code : ushort
    {
        RegisterAccount = 110,
        Login = 120,
    }

    public enum ERROR_CODE : ushort
    {
        SUCCESS = 0,

        FAIL_ETC = 10,

        #region ====:: 계정 관련 (1000~1100)::====
        REGISTER_FAIL_INVALID = 1015,
        REGISTER_FAIL_DUPLICATE = 1020,
        REGISTER_FAIL_UNKNOWN = 1025,

        LOGIN_FAIL_INVALID = 1030,
        LOGIN_FAIL_USER_NOT_EXIST = 1035,
        LOGIN_FAIL_PW_WRONG = 1040,
        #endregion
    }
}
