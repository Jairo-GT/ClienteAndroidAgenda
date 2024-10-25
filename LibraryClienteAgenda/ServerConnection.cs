using System.Net.Sockets;
using System.Text;


namespace LibraryClienteAgenda
{

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


    public class ServerConnection
    {
        private static string? ip;
        private static int port;
        private static string? token;
        private static UserInfo? connectedUser;
       

        public static string? Token { get => token; set => token = value; }
        public static UserInfo? ConnectedUser { get => connectedUser; set => connectedUser = value; }


        //UTILITY & SETUP
        
        /// <summary>
        /// Sets up the server IP address and port number for the connection.
        /// </summary>
        /// <param name="ip">The IP address of the server.</param>
        /// <param name="port">The port number of the server.</param>
        public static void SetupServerVars(string ip, int port)
        {

            ServerConnection.ip = ip;
            ServerConnection.port = port;
           
        }

        /// <summary>
        /// Envia una petición al servidor y espera una respuesta.
        /// </summary>
        /// <param name="dataToSend">Los petición al servidor.</param>
        /// <param name="timeoutMilliseconds">El tiempo de espera a un mensaje de respuesta o se cancela la tarea.</param>
        public static async Task<string> SendDataAsync(string dataToSend, int timeoutMilliseconds = 5000)
        {
            try
            {



                TcpClient tcpClient = new TcpClient() { NoDelay = true };


                using var cts = new CancellationTokenSource(timeoutMilliseconds);
                Task? connectTask = null;

                if (!tcpClient.Connected)
                {
                    connectTask = tcpClient.ConnectAsync(ip, port);
                }


                if (connectTask != null && await Task.WhenAny(connectTask, Task.Delay(timeoutMilliseconds, cts.Token)) != connectTask)
                {
                    throw new TimeoutException("Se agotó el tiempo de espera.");
                }

                using NetworkStream networkStream = tcpClient.GetStream();
                networkStream.WriteTimeout = timeoutMilliseconds;
                networkStream.ReadTimeout = timeoutMilliseconds;

                //Añadimos fin de linea para que el server pueda leer los datos
                byte[] data = Encoding.UTF8.GetBytes(dataToSend + "\n");

                await networkStream.WriteAsync(data);

                await networkStream.FlushAsync();

                byte[] responseBuffer = new byte[1024];
                int bytesRead = await networkStream.ReadAsync(responseBuffer, cts.Token);

                string response = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);


                return response;
            }
            catch (Exception ex)
            {

                return $"Error: {ex.Message}";
            }
        }

        /// <summary>
        /// Ensambla un mensaje de petición al servidor.
        /// </summary>
        /// <param name="protocol">El protocolo del mensaje.</param>
        /// <param name="action">La acción de la petición al servidor.</param>
        /// <param name="dataToSend">Los datos extras a enviar de manera secuencial en una List<string>.</param>
        /// <param name="encrypt">Si el mensaje al servidor debe estar encriptado.</param>
        public static string AssembleServerMessage(string protocol, string action, List<string> dataToSend, bool encrypt = false)
        {
            string message = protocol + action;

            foreach (var entry in dataToSend)
            {


                message += string.Format("{0:D2}", entry.Length) + entry;
            }


            return message;


        }

        /// <summary>
        /// Devuelve un diccionario con los datos parseados de una respuesta de servidor.
        /// </summary>
        /// <param name="keys">Las keys del diccionario devuelto despues de extraer los datos. Los extrae de manera secuencial.</param>
        /// <param name="data">Los datos a extraer formados por un offset que marca el tamaño y el dato en si en una String.</param>
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

        //HANDLE RESPONSES & ACTIONS
        /// <summary>
        /// Procesa una respuesta de servidor.
        /// </summary>
        /// <param name="response">La respuesta del servidor.</param>
        /// <returns>True si la respuesta es exitosa, False si hubo algun fallo o error.</returns>
        public static bool HandleResponse(string response)
        {
            string data;

            _ = int.TryParse(response[..1], out int parsedProtocol);

            int parsedAction;
            //Los otros son siempre 2 bytes para la acción pero en el 9 puede haber más (4 creo segun la wiki).
            if (parsedProtocol != 9)
            {

                _ = int.TryParse(response.AsSpan(1, 2), out parsedAction);
                data = response[3..];
            }
            else
            {

                _ = int.TryParse(response.AsSpan(1, 4), out parsedAction);
                data = response[5..];

            }
            System.Console.WriteLine("ParsedProtocol: " + parsedProtocol + "  ||  ParsedAction:" + parsedAction);
            System.Console.WriteLine("DATA: " + data);
            Protocol protocol = (Protocol)parsedProtocol;


            switch (protocol)
            {

                case Protocol.USER:
                    ServerUserActions serverUserAction = (ServerUserActions)parsedAction;
                    return HandleServerUserActions(serverUserAction, data);


                case Protocol.LOGIN:
                    ServerLoginActions serverLoginAction = (ServerLoginActions)parsedAction;
                    return HandleServerLoginActions(serverLoginAction, data);

                case Protocol.ERROR:
                    ServerErrorActions serverErrorAction = (ServerErrorActions)parsedAction;
                    return HandleServerErrorActions(serverErrorAction, data);
                default:
                    Console.WriteLine("Protocolo desconocido.");
                    break;
            }

            return false;

        }

        /// <summary>
        /// Procesa una acción del protocolo de servidor de ERROR.
        /// </summary>
        /// <param name="serverAction">La acción a confirmar.</param>
        /// <param name="data">Los datos de la respuesta del servidor.</param>
        /// <returns>La respuesta, siempre devuelve True.</returns>
        private static bool HandleServerErrorActions(ServerErrorActions serverAction, string data)
        {

            switch (serverAction)
            {

                case ServerErrorActions.DATA_BORN_GREATER_NOW:
                    Console.WriteLine("La data es major que la data actual.");
                    break;
                case ServerErrorActions.FORMAT_ERROR_PACKET:
                    Console.WriteLine("Error en el formato de paquete enviado al servidor.");
                    break;
                case ServerErrorActions.FULLNAME_IS_EMPTY:
                    Console.WriteLine("El nombre real no puede estar en blanco.");
                    break;
                case ServerErrorActions.LOGIN_FAILED:
                    Console.WriteLine("Los credenciales no son válidos.");
                    break;
                case ServerErrorActions.NOT_ALLOWED:
                    Console.WriteLine("Acción no permitida.");
                    break;
                case ServerErrorActions.UNKNOWN_ERROR:
                    Console.WriteLine("Error desconocodio en el servidor.");
                    break;
                case ServerErrorActions.USER_NAME_ALREADY_EXISTS:
                    Console.WriteLine("El nombre de usuario ya existe.");
                    break;
                case ServerErrorActions.USER_NAME_IS_EMPTY:
                    Console.WriteLine("El nombre de usuario no puede estar vacio.");
                    break;
                case ServerErrorActions.ALREADY_CONNECTED:
                    Console.WriteLine("Ya existe una conexión abierta, hay que hacer logout antes.");
                    break;

                default:
                    Console.WriteLine("Acción desconocida para ERROR_RESPONSES");
                    break;
            }

            return true;
        }

        /// <summary>
        /// Procesa una acción del protocolo de servidor de USER.
        /// </summary>
        /// <param name="serverAction">La acción a confirmar.</param>
        /// <param name="data">Los datos de la respuesta del servidor.</param>
        /// <returns>True si la acción fue exitosa y False de otra manera.</returns>
        private static bool HandleServerUserActions(ServerUserActions serverAction, string data)
        {

            switch (serverAction)
            {

                case ServerUserActions.USER_INFO:
                    Console.WriteLine("Handling USER INFO");
                    var dict = ParseData(["token", "user", "user2","userRol", "realName", "dateBorn"], data);
                    //Falta el alias pero hay algo raro como numeros de más en el mensaje
                    if (ConnectedUser != null)
                    {
                        ConnectedUser.IsAdmin = data.Trim().LastOrDefault().ToString() == "1";
                        ConnectedUser.FullName = dict["realName"];
                        ConnectedUser.DataNaixement = dict["dateBorn"];
                        ConnectedUser.Userrol = dict["userRol"];
                        ConnectedUser.UserToken = dict["token"];
                    }
                    else return false;
                    
                        return true;

                default:
                    Console.WriteLine("Acción desconocida para USER");
                    break;
            }

            return false;


        }
        
        /// <summary>
        /// Procesa una acción del protocolo de servidor de LOGIN.
        /// </summary>
        /// <param name="serverAction">La acción a confirmar.</param>
        /// <param name="data">Los datos de la respuesta del servidor.</param>
        /// <returns>True si la acción fue exitosa y False de otra manera.</returns>
        private static bool HandleServerLoginActions(ServerLoginActions serverAction, string data)
        {


            switch (serverAction)
            {
                case ServerLoginActions.LOGIN_SUCCESS:
                    Console.WriteLine("Handling LOGIN_SUCCESS");
                    var retToken = ServerConnection.ParseData(["token"], data);
                    if (retToken.TryGetValue("token", out string? value))
                    {

                        System.Console.WriteLine("TOKEN OBTAINED: " + value);
                        Token = value;
                        return true;
                    }
                    break;
                case ServerLoginActions.LOGOUT_SUCCESS:
                    Console.WriteLine("Handling LOGOUT_SUCCESS");
                    //NO HAY TOKEN DE MOMENTO ASI QUE.. return true;
                    ServerConnection.Token = "";
                    ServerConnection.ConnectedUser = null;
                    return true;            
                default:
                    Console.WriteLine("Acción desconocida para LOGIN");
                    break;
            }

            return false;
        }



        // DO ACTIONS
        /// <summary>
        /// Método para autenticarse en el servidor.
        /// </summary>
        /// <param name="user">El nombre de usuario.</param>
        /// <param name="password">La contraseña.</param>
        /// <returns>True si la acción fue exitosa y False de otra manera.</returns>
        public static async Task<bool> UserLogin(string user, string password)
        {
            ServerConnection.ConnectedUser = new(user);
            string loginData = ServerConnection.AssembleServerMessage(((int)Protocol.LOGIN).ToString(), ((int)ClientLoginActions.LOGIN).ToString("D2"), [user, password]);



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


        /// <summary>
        /// Método para deautenticarse en el servidor. Requiere un usuario autenticado.
        /// </summary>
        /// <returns>True si la acción fue exitosa y False de otra manera.</returns>
        public static async Task<bool> UserLogout()
        {



            if (!String.IsNullOrEmpty(ServerConnection.Token) && ConnectedUser != null)
            {


                string dataToSend = ServerConnection.AssembleServerMessage(((int)Protocol.LOGIN).ToString(), ((int)ClientLoginActions.LOGOUT).ToString("D2"), [ServerConnection.Token, ConnectedUser.UserName]);
                string response = await SendDataAsync(dataToSend);


                System.Console.WriteLine("SentLogout: " + dataToSend);
                System.Console.WriteLine("ResponseLogout:" + response);
                return HandleResponse(response);


            }


            return false;
        }


        /// <summary>
        /// Obtiene los datos de un usuario y los guarda en el campo ConnectedUser. Requiere un usuario autenticado.
        /// </summary>
        /// <returns>True si la acción fue exitosa y False de otra manera.</returns>
        public static async Task<bool> GetUserInfo()
        {

            if (String.IsNullOrEmpty(ServerConnection.Token) || ConnectedUser == null )
            {
                Console.WriteLine("GetuserInfo: El Token o Usuario conectado no son válidos.");
                return false;

            }

            string userInfo = ServerConnection.AssembleServerMessage(((int)Protocol.USER).ToString(), ((int)ClientUserActions.GET_USER_INFO).ToString("D2"), [ServerConnection.Token, ConnectedUser.UserName, ConnectedUser.UserName]);

            var response = await ServerConnection.SendDataAsync(userInfo);



            System.Console.WriteLine("SentGetUserInfo: " + userInfo);
            System.Console.WriteLine("ResponseGetUserInfo" + response);


            return HandleResponse(response);
        }

    }
}
