
namespace LibraryClienteAgenda
{


    public enum ResponseStatus
    {
        ACTION_SUCCESS = 1,
        ACTION_FAILED = 2,
        ACTION_ERROR = 3
    }
    public enum Protocol
    {
        LOGIN = 1,
        USER = 2,
        ERROR = 9
    }
    public enum ServerErrorActions
    {
        NOT_ALLOWED = 112,
        DATA_BORN_GREATER_NOW = 1105,
        FORMAT_ERROR_PACKET = 101,
        USER_NAME_ALREADY_EXISTS = 1202,
        USER_NAME_IS_EMPTY = 1201,
        FULLNAME_IS_EMPTY = 1203,
        LOGIN_FAILED = 109,
        ALREADY_CONNECTED = 113,
        EMPTY_PASSWORD = 114,
        UNKNOWN_ERROR = 9,
        ADMIN_IN_MODE_MANAGEMENT_ENABLED = 16,
        MANAGEMENT_MODE_ENABLED  = 17,
        MANAGEMENT_MODE_DISABLED = 18,  //??
        USER_DONT_EXISTS = 1205,
        PERMISSIONS_BASED_ON_ROL_DONT_EXISTS = 1301,
        PERMISSIONS_BASED_ON_ROL_ALREADY_EXISTS = 1302,
        PERMISSIONS_VALUES_ALREADY_EXISTS = 1303,
        GROUP_NAME_ALREADY_EXISTS = 1401


    }
    public enum ClientLoginActions
    {
        LOGIN = 1,
        LOGOUT = 2,
        SHUTDOWN_SERVER = 5

    }
    public enum ClientUserActions
    {
        CHANGE_PASSWORD = 1,
        CHANGE_FULLNAME = 2,
        CHANGE_BORN_DATE = 3,
        CHANGE_OTHER_DATA = 4,
        GET_USER_INFO = 5,
        ADD_NEW_USER = 6,
        DELETE_USER = 7,
        GET_ALL_USERS = 8,
        ADD_PERMISSION = 9,
        EDIT_PERMISSION = 10,
        DELETE_PERMISSION = 11,
        SHOW_PERMISSIONS = 12,
        ENABLE_MANAGEMENT_MODE = 13,
        DISABLE_MANAGEMENT_MODE = 14,
        CREATE_USER_GROUP = 15,
        DELETE_USER_GROUP = 16,
        ERASE_GROUP_AGENDA = 17,
        GET_USER_IS_OWNER_GROUPS = 18,
        GET_USER_IS_MEMBER_GROUPS = 19,
        GET_ALL_GROUPS = 20,
        INVITE_USER_TO_GROUP = 21

    }
    public enum ClientAgendaActions
    {
        GET_FROM_MONTH = 1,
        GET_NEXT_MONTH = 2,
        GET_PREVIOUS_MONTH = 3,
        GET_NEXT_YEAR = 4,
        GET_PREVIOUS_YEAR = 5,
        SET_DAY_EVENT_AND_TAGS = 6,
        MODIFY_DAY_CONTENT = 7,
        INSERT_DAY_TAG = 8,
        DELETE_DAY_TAG = 9,
        SHOW_EVENT_BY_TAG = 10,
        BLOCK_DAY_FOR_GROUP = 11,



    }
    public enum ServerLoginActions
    {
        LOGIN_SUCCESS = 3,
        LOGOUT_SUCCESS = 2,
        RESET_PASSWORD_AFTER_LOGOUT = 4,


    }
    public enum ServerUserActions
    {
        PERMISSION_ADDED =12,
        PERMISSION_MODIFIED = 13,
        PERMISSION_DELETED = 14, //USUARIO COMO EL 16???
        USER_INFO = 15,
        USER_ACCOUNT_DELETED = 16,
        PERMISSION_TO_SHOW = 17,
        GROUP_DELETED = 18,
        GROUP_DELETED_MESSAGE_MEMBERS = 19,
        CANVI_NOM = 20,
        CANVI_DATA = 21,
        CANVI_ALTRES = 22,
        NEW_USER_ADDED = 23,
        USER_DELETED = 24,  //USUARIO COMO EL 16???
        GROUP_AGENDA_DELETED = 25,
        GROUP_AGENDA_CREATED = 26



    }

}
