
using Android.Webkit;
using System.Net.Sockets;
using System.Text;



namespace PruebaMauiAndroid.Models
{
    class ServerConnection
    {
        private static string ip;
        private static int port;
        public static string token;
        public static UserInfo ConnectedUser;


        public enum Protocol
        {
            SERVER_LOGIN_RESPONSES = 1,
            SERVER_USER_RESPONSES = 2,
            SERVER_ERROR = 9
        }

        public enum ServerLoginActions
        {
            LOGIN_SUCCESS = 3,
            LOGIN_FAILED = 2,
            RESET_PASSWORD_AFTER_LOGOUT = 4,
        

        }

        public enum ServerUserActions
        { 
            USER_INFO = 15,
            CANVI_NOM = 20,
            CANVI_DATA = 21,
            CANVI_ALTRES = 22


        }


        public static string ConstructServerMessage(string protocol, string action, List<string> dataToSend)
        {
            string message = protocol + action;

            foreach (var entry in dataToSend)
            {


                message += string.Format("{0:D2}", entry.Length) + entry;
            }


            return message;


        }



        public static Dictionary<string, string> ParseData(List<string> keys, string data)
        {

            Dictionary<string, string> parsedData = [];

            foreach (var k in keys)
            {

                if (int.TryParse(data[..2], out int size))
                {

                    string extracted = data.Substring(2, size);

                    parsedData[k] = extracted;

                    data = data[(2 + size)..];

                }


            }


            return parsedData;

        }


        public static bool HandleResponse(string response)
        {

            int parsedProtocol = -1;
            int parsedAction = -1;

            int.TryParse(response[..1], out parsedProtocol);
            int.TryParse(response.Substring(1, 2), out parsedAction);
            string data = response.Substring(3);

            System.Console.WriteLine("ParsedProtocol: " + parsedProtocol + "  ||  ParsedAction:" + parsedAction);
            System.Console.WriteLine("DATA: " + data);
            Protocol protocol = (Protocol)parsedProtocol;


            switch (protocol)
            {

                case Protocol.SERVER_USER_RESPONSES:
                    ServerUserActions serverUserAction = (ServerUserActions)parsedAction;
                    return HandleServerUserActions(serverUserAction, data);
                 

                case Protocol.SERVER_LOGIN_RESPONSES:
                    ServerLoginActions serverLoginAction = (ServerLoginActions)parsedAction;
                    return HandleServerLoginActions(serverLoginAction, data);

                case Protocol.SERVER_ERROR:
                    Console.WriteLine("PROTOCOL 9 NOT HANDLED YET.");
                    return false;
                default:
                    Console.WriteLine("Protocolo desconocido.");
                    break;
            }

            return false;

        }

        private static bool HandleServerUserActions(ServerUserActions serverAction, string data)
        {

            switch (serverAction)
            {
                
                case ServerUserActions.USER_INFO:
                    Console.WriteLine("Handling USER INFO");
                    var dict = ParseData(["token", "user", "user2", "realName", "fecha"], data);
                    //Falta el alias pero hay algo raro como numeros de más en el mensaje
                    ConnectedUser.isAdmin = data.Trim().LastOrDefault().ToString() == "1";
                    return true;

                default:
                    Console.WriteLine("Acción desconocida para SERVER_USER_RESPONSES");
                    break;
            }

            return false;


        }

        private static bool HandleServerLoginActions(ServerLoginActions serverAction, string data)
        {


            switch (serverAction)
            {
                case ServerLoginActions.LOGIN_SUCCESS:
                    Console.WriteLine("Handling LOGIN_SUCCESS");
                    var retToken = ServerConnection.ParseData(["token"], data);
                    if (retToken.ContainsKey("token"))
                    {

                        System.Console.WriteLine("TOKEN OBTAINED: " + retToken["token"]);
                        ServerConnection.token = retToken["token"];
                        return true;
                    }
                    break;
                case ServerLoginActions.LOGIN_FAILED:
                    Console.WriteLine("Handling LOGIN_FAILED");
                    break;
              

                default:
                    Console.WriteLine("Acción desconocida para SERVER_LOGIN_RESPONSES");
                    break;
            }

            return false;
        }



        public static async Task<bool> UserLogin(string user, string password)
        {
            ServerConnection.ConnectedUser = new(user);
            string loginData = ServerConnection.ConstructServerMessage("1", "01", [user, password]);



            var response = await ServerConnection.SendDataAsync(loginData);

            System.Console.WriteLine("SentLogin: " + loginData);
            System.Console.WriteLine("ResponseLogin:" + response);

            var isValidResponse = HandleResponse(response);


            if (isValidResponse)
            {
                await GetUserInfo();
            }


            return isValidResponse;
        }

        public static async Task<bool> UserLogout()
        {



            if (!String.IsNullOrEmpty(ServerConnection.token))
            {

                string dataToSend = ServerConnection.ConstructServerMessage("1", "05", [ServerConnection.token, ConnectedUser.userName]);
                string response = await SendDataAsync(dataToSend);


                System.Console.WriteLine("SentLogout: " + dataToSend);
                System.Console.WriteLine("ResponseLogout:" + response);
                return HandleResponse(response);


            }


            return false;
        }

        public static async Task<bool> GetUserInfo()
        {

            string userInfo = ServerConnection.ConstructServerMessage("2", "05", [ServerConnection.token, ConnectedUser.userName, ConnectedUser.userName]);

            var response = await ServerConnection.SendDataAsync(userInfo);



            System.Console.WriteLine("SentGetUserInfo: " + userInfo);
            System.Console.WriteLine("ResponseGetUserInfo" + response);


            return HandleResponse(response);
        }

        public ServerConnection(string ip, int port)
        {

            ServerConnection.ip = ip;
            ServerConnection.port = port;


        }





        public static async Task<string> SendDataAsync(string dataToSend, int timeoutMilliseconds = 5000)
        {
            try
            {

                using TcpClient tcpClient = new()
                {
                    NoDelay = true
                };



                using var cts = new CancellationTokenSource(timeoutMilliseconds);

                Task connectTask = tcpClient.ConnectAsync(ip, port);
                if (await Task.WhenAny(connectTask, Task.Delay(timeoutMilliseconds, cts.Token)) != connectTask)
                {
                    throw new TimeoutException("Se agotó el tiempo de espera.");
                }

                using NetworkStream networkStream = tcpClient.GetStream();
                networkStream.WriteTimeout = timeoutMilliseconds;
                networkStream.ReadTimeout = timeoutMilliseconds;

                //Añadimos fin de linea para que el server pueda leer los datos
                byte[] data = Encoding.UTF8.GetBytes(dataToSend + "\n");

                await networkStream.WriteAsync(data, 0, data.Length);

                await networkStream.FlushAsync();

                byte[] responseBuffer = new byte[1024];
                int bytesRead = await networkStream.ReadAsync(responseBuffer, 0, responseBuffer.Length, cts.Token);

                string response = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);

                return response;
            }
            catch (Exception ex)
            {

                return $"Error: {ex.Message}";
            }
        }

    }
}
