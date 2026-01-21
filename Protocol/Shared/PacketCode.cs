
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

        ChangeSkill = 501,

        CreateStructure = 1000,
        DestroyStructure = 1001,
    }

    public enum ERROR_CODE : ushort
    {
        SUCCESS = 0,

        FAIL_INVALID_TOKEN = 5,

        FAIL_UNKNOWN = 10,

        FAIL_INVALID_USER = 20,

        FAIL_EMPTY_REQUEST = 50,

        #region ====:: 계정 관련 (1000~1100)::====
        REGISTER_FAIL_INVALID = 1015,
        REGISTER_FAIL_DUPLICATE = 1020,
        REGISTER_FAIL_UNKNOWN = 1025,

        LOGIN_FAIL_USER_NOT_EXIST = 1035,
        LOGIN_FAIL_PW_WRONG = 1040,
        #endregion

        NOT_ENOUGH_CURRENCY = 1500,

        #region ====:: 건물 관련 (2000~2500)::====
        CREATE_STRUCTURE_FAIL_01 = 2000,
        CREATE_STRUCTURE_FAIL_02 = 2001,
        #endregion
    }
}
