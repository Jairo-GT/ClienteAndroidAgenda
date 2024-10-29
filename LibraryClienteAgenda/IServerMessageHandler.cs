
namespace LibraryClienteAgenda
{
    public interface IServerMessageHandler
    {
        ResponseStatus HandleResponse<TAction>(ServerMessage<TAction> message) where TAction : Enum;
        Dictionary<string, string> ParseData(List<string> keys, string data, int offsetSize = 2, Dictionary<string, string>? parsedData = null);
        void SetConnection(IServerConnection serverInterface);
    }
}