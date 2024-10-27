using System.Net.Security;
using System.Net.Sockets;
using System.Text;


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


    public class ServerConnection
    {
        private static string? ip;
        private static int port;
        //private static string? token;
        private static UserInfo? connectedUser;
       

        //public static string? Token { get => token; set => token = value; }
        public static UserInfo? ConnectedUser { get => connectedUser; set => connectedUser = value; }


        #region HELPERS & SETUP

        /// <summary>
        /// Prepara los campos de IP y PUERTO para la conexión con el servidor.
        /// </summary>
        /// <param name="ip">La dirección IP del servidor.</param>
        /// <param name="port">El puerto del servidor.</param>
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
        /// <returns>La respuesta del servidor.</returns>
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

                //Añadimos fin de linea ya que es lo que el server espera como fin de mensaje.
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
        /// <returns>Una string que contiene la petición para el servidor construida.</returns>
        public static string AssembleServerMessage(string protocol, string action, List<string> dataToSend, bool encrypt = false)
        {
            string message = protocol + action;

            foreach (var entry in dataToSend)
            {


                message += string.Format("{0:D2}", entry.Length) + entry;
            }


            return message;


        }
        private static string Add4bytesFields(List<string> dataToSend,string message = "")
        {

            foreach (var entry in dataToSend)
            {


                message += string.Format("{0:D4}", entry.Length) + entry;
            }

            return message;

        }

        /// <summary>
        /// Devuelve un diccionario con los datos parseados de una respuesta de servidor.
        /// </summary>
        /// <param name="keys">Las keys del diccionario devuelto despues de extraer los datos. Los extrae de manera secuencial.</param>
        /// <param name="data">Los datos a extraer formados por un offset que marca el tamaño y el dato en si en una String.</param>
        /// <param name="offsetSize">Tamaño del offset en bytes antes de cada campo sucesivo.</param>
        /// <param name="offsetSize">Diccionario con los datos segregados, si es null creara uno nuevo si los agregara al diccionario y lo devolverá.</param>
        /// <returns>Un diccionario con los campos indicados extraidos de la string data. Si hay alguna excepción devolverá un diccionario vacío.</returns>
        public static Dictionary<string, string> ParseData(List<string> keys, string data, int offsetSize = 2, Dictionary<string,string>? parsedData = null)
        {
            try
            {
                parsedData = parsedData ?? new Dictionary<string, string>();

                Console.WriteLine($"DataToParse: {data}");
                foreach (var k in keys)
                {

                    if (int.TryParse(data[..offsetSize], out int size))
                    {

                        string extracted = data.Substring(offsetSize, size);

                        parsedData[k] = extracted;

                        data = data[(offsetSize + size)..];

                    }


                }

                parsedData["nonParsedData"] = data;
                Console.WriteLine($"NonParsedData: {data}");

                return parsedData;

            }
            catch (Exception _)
            {
                parsedData = null;
                return parsedData;

            }
           
          

        }

        //HANDLE RESPONSES & ACTIONS
        /// <summary>
        /// Procesa una respuesta de servidor.
        /// </summary>
        /// <param name="response">La respuesta del servidor.</param>
        /// <returns>Devuelve un ResponseStatus con SUCCES,FAILED,ERROR segun si la acción tuvo éxito,falló o devolvió un mensaje de error/denegación. </returns>
        public static ResponseStatus HandleResponse(string response)
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

            return ResponseStatus.ACTION_FAILED;

        }

        /// <summary>
        /// Procesa una acción del protocolo de servidor de ERROR.
        /// </summary>
        /// <param name="serverAction">La acción a confirmar.</param>
        /// <param name="data">Los datos de la respuesta del servidor.</param>
        /// <returns>Devuelve un ResponseStatus con ERROR o FAILED si la acción no fue gestionada. </returns>
        private static ResponseStatus HandleServerErrorActions(ServerErrorActions serverAction, string data)
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
                    return ResponseStatus.ACTION_FAILED;
                    
            }

            return ResponseStatus.ACTION_ERROR;
        }

        /// <summary>
        /// Procesa una acción del protocolo de servidor de USER.
        /// </summary>
        /// <param name="serverAction">La acción a confirmar.</param>
        /// <param name="data">Los datos de la respuesta del servidor.</param>
        /// <returns>Devuelve un ResponseStatus con SUCCES si tuvo éxito o FAILED si la acción no fue gestionada.</returns>
        private static ResponseStatus HandleServerUserActions(ServerUserActions serverAction, string data)
        {

            switch (serverAction)
            {

                case ServerUserActions.USER_INFO:
                    Console.WriteLine("Handling USER INFO");
                    var userInfoData = ParseData(["token", "user","userRol", "realName", "dateBorn"], data);

                    if (userInfoData != null && ConnectedUser != null)
                    {
                        ConnectedUser.IsAdmin = data.Trim().LastOrDefault().ToString() == "1";
                        ConnectedUser.FullName = userInfoData["realName"];
                        ConnectedUser.DataNaixement = userInfoData["dateBorn"];
                        ConnectedUser.Userrol = userInfoData["userRol"];
                        ConnectedUser.Token = userInfoData["token"];
                        var parsedData = ParseData(["extraData"], userInfoData["nonParsedData"], 4, userInfoData);
                        ConnectedUser.DatosExtra = parsedData != null && parsedData.TryGetValue("extraData", out var extraData)? extraData : "";
                    }
                    else return ResponseStatus.ACTION_FAILED;
                    
                        return ResponseStatus.ACTION_SUCCESS;
                case ServerUserActions.CANVI_NOM:
                    Console.WriteLine("Handling CHANGE_FULLNAME");
                    var ChangeFullnameData = ParseData(["token", "user", "userRol", "realName", "dateBorn"], data);
                    if (ChangeFullnameData != null && ConnectedUser != null)
                    {
                        if(ConnectedUser.Token == ChangeFullnameData["token"])
                        return ResponseStatus.ACTION_SUCCESS;
                    }
                        break;
                case ServerUserActions.CANVI_DATA:
                    Console.WriteLine("Handling CHANGE_DATE_BORN");
                    var ChangeDateData = ParseData(["token", "user", "userRol", "realName", "dateBorn"], data);
                    if (ChangeDateData != null && ConnectedUser != null)
                    {
                        if (ConnectedUser.Token == ChangeDateData["token"])
                            return ResponseStatus.ACTION_SUCCESS;
                    }
                    break;
                case ServerUserActions.CANVI_ALTRES:
                    Console.WriteLine("Handling CHANGE_DETAILS");
                    var ChangeDetailsData = ParseData(["token", "user", "userRol", "realName", "dateBorn"], data);
                    if (ChangeDetailsData != null && ConnectedUser != null)
                    {
                        if (ConnectedUser.Token == ChangeDetailsData["token"])
                            return ResponseStatus.ACTION_SUCCESS;
                    }
                    break;
                default:
                    Console.WriteLine("Acción desconocida para USER");
                    break;
            }

            return ResponseStatus.ACTION_FAILED;


        }
        
        /// <summary>
        /// Procesa una acción del protocolo de servidor de LOGIN.
        /// </summary>
        /// <param name="serverAction">La acción a confirmar.</param>
        /// <param name="data">Los datos de la respuesta del servidor.</param>
        /// <returns>Devuelve un ResponseStatus con SUCCES si tuvo éxito o FAILED si la acción no fue gestionada.</returns>
        private static ResponseStatus HandleServerLoginActions(ServerLoginActions serverAction, string data)
        {


            switch (serverAction)
            {
                case ServerLoginActions.LOGIN_SUCCESS:
                    Console.WriteLine("Handling LOGIN_SUCCESS");
                    var retTokenLogin = ServerConnection.ParseData(["token"], data);
                    if (ConnectedUser!=null && retTokenLogin != null && retTokenLogin.TryGetValue("token", out string? valueTokenLogin))
                    {

                        System.Console.WriteLine("TOKEN OBTAINED: " + valueTokenLogin);
                        ConnectedUser.Token = valueTokenLogin;
                        return ResponseStatus.ACTION_SUCCESS;
                    }
                   
                    break;
                case ServerLoginActions.LOGOUT_SUCCESS:
                    Console.WriteLine("Handling LOGOUT_SUCCESS");
                    var retTokenLogout = ServerConnection.ParseData(["token"], data);
                    if (ConnectedUser != null && retTokenLogout != null && retTokenLogout.TryGetValue("token", out string? valueTokenLogout))
                    {
                        Console.WriteLine($"TokenInResponse:{valueTokenLogout}");
                        Console.WriteLine($"TokenCurrentlyActive:{ServerConnection.ConnectedUser.Token}");

                        if (ConnectedUser != null && ServerConnection.ConnectedUser.Token != null && valueTokenLogout == ServerConnection.ConnectedUser.Token)
                        {
                            ServerConnection.ConnectedUser.Token = "";
                            ServerConnection.ConnectedUser = null;
                            return ResponseStatus.ACTION_SUCCESS;
                        }

                    }
                    break;
                case ServerLoginActions.RESET_PASSWORD_AFTER_LOGOUT:
                    Console.WriteLine("Handling CHANGE_PASSWORD");
                    var retTokenChangePassword = ServerConnection.ParseData(["token"], data);
                    if (ConnectedUser != null && retTokenChangePassword != null && retTokenChangePassword.TryGetValue("token", out string? valueTokenChangePassword))
                    {

                        if (ServerConnection.ConnectedUser.Token != null && valueTokenChangePassword == ServerConnection.ConnectedUser.Token)
                        {

                            return ResponseStatus.ACTION_SUCCESS;
                        
                        }

                        }
                        break;

                        default:
                    Console.WriteLine("Acción desconocida para LOGIN");
                    break;
            }

            return ResponseStatus.ACTION_FAILED;

        }

        #endregion



        #region SERVER ACTIONS
        /// <summary>
        /// Método para autenticarse en el servidor.
        /// </summary>
        /// <param name="user">El nombre de usuario.</param>
        /// <param name="password">La contraseña.</param>
        /// <returns>True si la acción fue exitosa y False de otra manera.</returns>
        public static async Task<ResponseStatus> UserLogin(string user, string password)
        {
            ServerConnection.ConnectedUser = new(user);
            string loginData = ServerConnection.AssembleServerMessage(((int)Protocol.LOGIN).ToString(), ((int)ClientLoginActions.LOGIN).ToString("D2"), [user, password]);



            var response = await ServerConnection.SendDataAsync(loginData);

            System.Console.WriteLine("SentLogin: " + loginData);
            System.Console.WriteLine("ResponseLogin:" + response);

            var isValidResponse = HandleResponse(response);


            if (isValidResponse == ResponseStatus.ACTION_SUCCESS)
            {
                await GetUserInfo();
            }


            return isValidResponse;
        }


        /// <summary>
        /// Método para deautenticarse en el servidor. Requiere un usuario autenticado.
        /// </summary>
        /// <returns>True si la acción fue exitosa y False de otra manera.</returns>
        public static async Task<ResponseStatus> UserLogout()
        {



            if (ConnectedUser != null && !String.IsNullOrEmpty(ServerConnection.ConnectedUser.Token))
            {


                string dataToSend = ServerConnection.AssembleServerMessage(((int)Protocol.LOGIN).ToString(), ((int)ClientLoginActions.LOGOUT).ToString("D2"), [ServerConnection.ConnectedUser.Token, ConnectedUser.UserName]);
                string response = await SendDataAsync(dataToSend);


                System.Console.WriteLine("SentLogout: " + dataToSend);
                System.Console.WriteLine("ResponseLogout:" + response);
                return HandleResponse(response);


            }


            return ResponseStatus.ACTION_FAILED;
        }


        /// <summary>
        /// Obtiene los datos de un usuario y los guarda en el campo ConnectedUser. Requiere un usuario autenticado.
        /// </summary>
        /// <returns>True si la acción fue exitosa y False de otra manera.</returns>
        public static async Task<ResponseStatus> GetUserInfo()
        {

            if (String.IsNullOrEmpty(ServerConnection.ConnectedUser.Token) || ConnectedUser == null )
            {
                Console.WriteLine("GetuserInfo: El Token o Usuario conectado no son válidos.");
                return ResponseStatus.ACTION_FAILED;

            }

            string userInfo = ServerConnection.AssembleServerMessage(((int)Protocol.USER).ToString(), ((int)ClientUserActions.GET_USER_INFO).ToString("D2"), [ServerConnection.ConnectedUser.Token, ConnectedUser.UserName, ConnectedUser.UserName]);

            var response = await ServerConnection.SendDataAsync(userInfo);



            System.Console.WriteLine("SentGetUserInfo: " + userInfo);
            System.Console.WriteLine("ResponseGetUserInfo" + response);


            return HandleResponse(response);
        }



        /// <summary>
        /// Cambia la contraseña del usuario.
        /// </summary>
        ///<param name="newPassword">La nueva contraseña.</param>
        ///<param name="oldPassword">La antigua contraseña.</param>
        /// <returns>Deuvle ResponseStatus.ACTION_SUCCES si la acción fue exitosa .ACTION_ERROR si ocurrio algun error o.ACTION_FAILED si no se pudo procesar.</returns>
        public static async Task<ResponseStatus> ChangeUserPassword(string newPassword,string oldPassword) {

            if (ConnectedUser != null  && String.IsNullOrEmpty(ServerConnection.ConnectedUser.Token))
            {
                Console.WriteLine("GetuserInfo: El Token o Usuario conectado no son válidos.");
                return ResponseStatus.ACTION_FAILED;

            }

            string changePasswordMessage = AssembleServerMessage(((int)Protocol.USER).ToString(), ((int)ClientUserActions.CHANGE_PASSWORD).ToString("D2"), [ServerConnection.ConnectedUser.Token,ConnectedUser.UserName,oldPassword,newPassword,ConnectedUser.UserName]);
            string response = await SendDataAsync(changePasswordMessage);




            return HandleResponse(response);
        }


        /// <summary>
        /// Cambia el nombre completo de un usuario.
        /// </summary>
        ///<param name="newFullname">El nuevo nombre completo.</param>
        /// <returns>Deuvle ResponseStatus.ACTION_SUCCES si la acción fue exitosa .ACTION_ERROR si ocurrio algun error o.ACTION_FAILED si no se pudo procesar.</returns>
        public static async Task<ResponseStatus> ChangeUserFullName(string newFullname)
        {

            if (ConnectedUser != null && String.IsNullOrEmpty(ServerConnection.ConnectedUser.Token))
            {
                Console.WriteLine("ChangeUserFullName: El Token o Usuario conectado no son válidos.");
                return ResponseStatus.ACTION_FAILED;

            }

            string changeFullNameMessage = AssembleServerMessage(((int)Protocol.USER).ToString(), ((int)ClientUserActions.CHANGE_FULLNAME).ToString("D2"), [ServerConnection.ConnectedUser.Token, ConnectedUser.UserName,newFullname,ConnectedUser.UserName ]);
            Console.WriteLine($"Sending message: {changeFullNameMessage}");
            string response = await SendDataAsync(changeFullNameMessage);
           
            return HandleResponse(response);
        }

        /// <summary>
        /// Cambia la fecha de nacimiento de un usuario.
        /// </summary>
        ///<param name="newFullname">La nueva fecha.</param>
        /// <returns>Deuvle ResponseStatus.ACTION_SUCCES si la acción fue exitosa .ACTION_ERROR si ocurrio algun error o.ACTION_FAILED si no se pudo procesar.</returns>
        public static async Task<ResponseStatus> ChangeUserDateBorn(string newDateBorn)
        {

            if (ConnectedUser != null && String.IsNullOrEmpty(ServerConnection.ConnectedUser.Token))
            {
                Console.WriteLine("ChangeUserDateBorn: El Token o Usuario conectado no son válidos.");
                return ResponseStatus.ACTION_FAILED;

            }

            string changeDateBornMessage = AssembleServerMessage(((int)Protocol.USER).ToString(), ((int)ClientUserActions.CHANGE_BORN_DATE).ToString("D2"), [ServerConnection.ConnectedUser.Token, ConnectedUser.UserName,ConnectedUser.UserName,newDateBorn]);
            Console.WriteLine($"Sending message: {changeDateBornMessage}");
            string response = await SendDataAsync(changeDateBornMessage);

            return HandleResponse(response);
        }
      
        
        /// <summary>
        /// Cambia los detalles de un usuario.
        /// </summary>
        ///<param name="newDetails">Los nuevos detalles.</param>
        /// <returns>Deuvle ResponseStatus.ACTION_SUCCES si la acción fue exitosa .ACTION_ERROR si ocurrio algun error o.ACTION_FAILED si no se pudo procesar.</returns>
        public static async Task<ResponseStatus> ChangeUserDetails(string newDetails)
        {

            if (ConnectedUser != null && String.IsNullOrEmpty(ServerConnection.ConnectedUser.Token))
            {
                Console.WriteLine("ChangeUserDetails: El Token o Usuario conectado no son válidos.");
                return ResponseStatus.ACTION_FAILED;

            }

            string changeDetailsMessage = AssembleServerMessage(((int)Protocol.USER).ToString(), ((int)ClientUserActions.CHANGE_OTHER_DATA).ToString("D2"), [ServerConnection.ConnectedUser.Token, ConnectedUser.UserName, ConnectedUser.UserName]);
            changeDetailsMessage = Add4bytesFields([newDetails], changeDetailsMessage);
            
            Console.WriteLine($"Sending message: {changeDetailsMessage}");
            string response = await SendDataAsync(changeDetailsMessage);

            return HandleResponse(response);
        }

        #endregion
    }
}
