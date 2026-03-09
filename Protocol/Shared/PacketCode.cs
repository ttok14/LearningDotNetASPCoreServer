
namespace JNetwork
{
    // 패킷 아이디  
    public enum Code : ushort
    {
        CheckVersion = 100,

        RegisterAccount = 110,
        Login = 120,

        EnterNickname = 130,
        SetStatusMessage = 140,

        CheatAddCurrency = 200,

        EquipDeploymentSlot = 300,
        UnequipDeploymentSlot = 310,

        EquipHero = 400,
        UnequipHero = 410,

        ChangeSkill = 501,

        CreateStructure = 1000,
        DestroyStructure = 1001,

        GarrisonUnit = 1500,
        SetSpawnUnit = 1550,
        UngarrisonUnit = 1600,

        EnterHome = 2000,
        SearchOpponent = 2500,

        BuyItem = 3000,

        MoveEntity = 4000,
    }

    public enum ERROR_CODE : ushort
    {
        SUCCESS = 0,

        EXCEPTION = 2,

        INVALID_INPUT = 5,

        FAIL_DATABASE_SAVE = 6,
        FAIL_TABLE_DATA_NO_EXIST = 7,

        FAIL_SERVER_ERROR = 8,

        FAIL_MAINTENANCE = 10,
        FAIL_INVALID_APP_VERSION = 13,

        FAIL_INVALID_TOKEN = 15,

        FAIL_UNKNOWN = 30,

        FAIL_INVALID_USER = 40,

        FAIL_EMPTY_REQUEST = 50,

        #region ====:: 테이블 관련 ::====
        NOT_EXIST_IN_TABLE = 800,

        NOT_CHARACTER_TYPE = 810,
        NOT_STRUCTURE_TYPE = 820,

        NOT_SPAWNER_TYPE = 825,

        ITEM_NOT_SQUAD_TYPE = 830,
        NOT_ITEM_HERO_TYPE = 840,
        #endregion

        #region ====:: 계정 관련 (1000~1100)::====
        REGISTER_FAIL_INVALID = 1015,
        REGISTER_FAIL_DUPLICATE = 1020,
        REGISTER_FAIL_UNKNOWN = 1025,
        REGISTER_ID_OR_PW_EMPTY = 1030,

        REGISTER_USER_HERO_ISSUE = 1032,

        LOGIN_FAIL_USER_NOT_EXIST = 1035,
        LOGIN_ID_WRONG = 1030,
        LOGIN_PW_WRONG = 1040,

        NICKNAME_EMPTY = 1044,
        NICKNAME_TOO_SHORT = 1045,
        NICKNAME_TOO_LONG = 1046,
        NICKNAME_ALREADY_SET = 1047,
        NICKNAME_DUPLICATE = 1048,
        NICKNAME_INVALID_CHARACTER = 1049,
        NICKNAME_START_WITH_DIGIT = 1050,
        NICKNAME_RULE_VIOLATION = 1051,
        #endregion

        #region ====:: 재화 관련 (1500~1600) ::====
        NOT_ENOUGH_CURRENCY = 1500,
        INVALID_CURRENCY_TYPE = 1505,
        #endregion

        #region ====:: 건물 관련 (2000~2500)::====
        CREATE_STRUCTURE_FAIL_01 = 2000,
        CREATE_STRUCTURE_FAIL_02 = 2001,

        NO_TARGET_ENTITY_FOUND = 2050,
        NO_GARRISON_UNIT_FOUND = 2055,

        GARRISON_SLOT_OUT_OF_INDEX = 2060,
        #endregion

        #region ====:: 매칭 관련 (3000~3500)::====
        SEARCH_OPPONENT_NOT_FOUND = 3000,
        #endregion

        #region ====:: 아이템 / 장착 관련 (4000~4500)::====
        INVALID_ITEM = 4000,
        ITEM_ALREADY_OWNED = 4010,
        PURCHASE_COST_NOT_FOUND = 4020,
        ITEM_NO_EXIST = 4050,
        SLOT_OUT_OF_RANGE = 4080,
        SLOT_NOT_EXIST = 4090,
        NO_ITEM_TO_UNEQUIP = 4100,
        #endregion

    }
}
