
namespace LibraryClienteAgenda
{
    public interface IServerConnection
    {
        UserInfo? ConnectedUser { get; set; }

        Task<ResponseStatus> ChangeUserDateBorn(string newDateBorn);
        Task<ResponseStatus> ChangeUserDetails(string newDetails);
        Task<ResponseStatus> ChangeUserFullName(string newFullname);
        Task<ResponseStatus> ChangeUserPassword(string newPassword, string oldPassword);
        Task<ResponseStatus> GetUserInfo();
        Task<ServerMessage<TAction>?> SendDataAsync<TAction>(ServerMessage<TAction> message, int timeoutMilliseconds = 5000) where TAction : Enum;
        void SetupServerVars(string ip, int port);
        Task<ResponseStatus> UserLogin(string user, string password);
        Task<ResponseStatus> UserLogout();
    }
}