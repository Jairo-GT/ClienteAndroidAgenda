
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
        UNKNOWN_ERROR = 9

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
        GET_USER_INFO = 5

    }
    public enum ServerLoginActions
    {
        LOGIN_SUCCESS = 3,
        LOGOUT_SUCCESS = 2,
        RESET_PASSWORD_AFTER_LOGOUT = 4,


    }
    public enum ServerUserActions
    {
        USER_INFO = 15,
        CANVI_NOM = 20,
        CANVI_DATA = 21,
        CANVI_ALTRES = 22


    }

}
