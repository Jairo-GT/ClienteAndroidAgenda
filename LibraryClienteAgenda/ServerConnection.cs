using System.Net.Security;
using System.Net.Sockets;
using System.Text;


namespace LibraryClienteAgenda
{
 

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

        #endregion

        public static async Task<ServerMessage<TAction>?> SendDataAsync<TAction>(ServerMessage<TAction> message, int timeoutMilliseconds = 5000) where TAction: Enum
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
                    throw new TimeoutException("Couldn't connect with the server, please try again later.");
                }

                using NetworkStream networkStream = tcpClient.GetStream();
                networkStream.WriteTimeout = timeoutMilliseconds;
                networkStream.ReadTimeout = timeoutMilliseconds;

                //Añadimos fin de linea ya que es lo que el server espera como fin de mensaje.
                byte[] data = Encoding.UTF8.GetBytes(message.RequestMessage + "\n");

                await networkStream.WriteAsync(data);

                await networkStream.FlushAsync();

                Console.WriteLine($"MESSAGE SENT:  {message.RequestMessage}");

                byte[] responseBuffer = new byte[1024];
                int bytesRead = await networkStream.ReadAsync(responseBuffer, cts.Token);

                string response = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);

                message.DissasembleResponse(response);
                return message;
            }
            catch (Exception)
            {

                Console.WriteLine("SendDataAsync unhandled error.");
                return null;
            }
        }



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
            var message = await ServerConnection.SendDataAsync(ServerMessageFactory.Create(Protocol.LOGIN, ClientLoginActions.LOGIN, [user,password]));


            var isValidResponse = ServerMessageHandler.HandleResponse(message);


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


                var message = await SendDataAsync(ServerMessageFactory.Create(Protocol.LOGIN, ClientLoginActions.LOGOUT, [ServerConnection.ConnectedUser.Token, ConnectedUser.UserName]));

                return ServerMessageHandler.HandleResponse(message);


            }


            return ResponseStatus.ACTION_FAILED;
        }


        /// <summary>
        /// Obtiene los datos de un usuario y los guarda en el campo ConnectedUser. Requiere un usuario autenticado.
        /// </summary>
        /// <returns>True si la acción fue exitosa y False de otra manera.</returns>
        public static async Task<ResponseStatus> GetUserInfo()
        {

            if (ConnectedUser == null || String.IsNullOrEmpty(ServerConnection.ConnectedUser.Token)  )
            {
                Console.WriteLine("GetuserInfo: El Token o Usuario conectado no son válidos.");
                return ResponseStatus.ACTION_FAILED;

            }
        
            var message = await ServerConnection.SendDataAsync(ServerMessageFactory.Create(Protocol.USER, ClientUserActions.GET_USER_INFO, [ServerConnection.ConnectedUser.Token, ConnectedUser.UserName, ConnectedUser.UserName]));


            return ServerMessageHandler.HandleResponse(message);
        }



        /// <summary>
        /// Cambia la contraseña del usuario.
        /// </summary>
        ///<param name="newPassword">La nueva contraseña.</param>
        ///<param name="oldPassword">La antigua contraseña.</param>
        /// <returns>Deuvle ResponseStatus.ACTION_SUCCES si la acción fue exitosa .ACTION_ERROR si ocurrio algun error o.ACTION_FAILED si no se pudo procesar.</returns>
        public static async Task<ResponseStatus> ChangeUserPassword(string newPassword,string oldPassword) {

            if (ConnectedUser == null  || String.IsNullOrEmpty(ServerConnection.ConnectedUser.Token))
            {
                Console.WriteLine("GetuserInfo: El Token o Usuario conectado no son válidos.");
                return ResponseStatus.ACTION_FAILED;

            }

            var message = await SendDataAsync(ServerMessageFactory.Create(Protocol.USER, ClientUserActions.CHANGE_PASSWORD, [ServerConnection.ConnectedUser.Token, ConnectedUser.UserName, oldPassword, newPassword, ConnectedUser.UserName]));




            return ServerMessageHandler.HandleResponse(message);
        }


        /// <summary>
        /// Cambia el nombre completo de un usuario.
        /// </summary>
        ///<param name="newFullname">El nuevo nombre completo.</param>
        /// <returns>Deuvle ResponseStatus.ACTION_SUCCES si la acción fue exitosa .ACTION_ERROR si ocurrio algun error o.ACTION_FAILED si no se pudo procesar.</returns>
        public static async Task<ResponseStatus> ChangeUserFullName(string newFullname)
        {

            if (ConnectedUser == null || String.IsNullOrEmpty(ServerConnection.ConnectedUser.Token))
            {
                Console.WriteLine("ChangeUserFullName: El Token o Usuario conectado no son válidos.");
                return ResponseStatus.ACTION_FAILED;

            }

            var message = await SendDataAsync(ServerMessageFactory.Create(Protocol.USER, ClientUserActions.CHANGE_FULLNAME, [ServerConnection.ConnectedUser.Token, ConnectedUser.UserName, newFullname, ConnectedUser.UserName]));
           
            return ServerMessageHandler.HandleResponse(message);
        }

        /// <summary>
        /// Cambia la fecha de nacimiento de un usuario.
        /// </summary>
        ///<param name="newFullname">La nueva fecha.</param>
        /// <returns>Deuvle ResponseStatus.ACTION_SUCCES si la acción fue exitosa .ACTION_ERROR si ocurrio algun error o.ACTION_FAILED si no se pudo procesar.</returns>
        public static async Task<ResponseStatus> ChangeUserDateBorn(string newDateBorn)
        {

            if (ConnectedUser == null || String.IsNullOrEmpty(ServerConnection.ConnectedUser.Token))
            {
                Console.WriteLine("ChangeUserDateBorn: El Token o Usuario conectado no son válidos.");
                return ResponseStatus.ACTION_FAILED;

            }

            var message = await SendDataAsync(ServerMessageFactory.Create(Protocol.USER, ClientUserActions.CHANGE_BORN_DATE, [ServerConnection.ConnectedUser.Token, ConnectedUser.UserName, ConnectedUser.UserName, newDateBorn]));

            return ServerMessageHandler.HandleResponse(message);
        }
      
        
        /// <summary>
        /// Cambia los detalles de un usuario.
        /// </summary>
        ///<param name="newDetails">Los nuevos detalles.</param>
        /// <returns>Deuvle ResponseStatus.ACTION_SUCCES si la acción fue exitosa .ACTION_ERROR si ocurrio algun error o.ACTION_FAILED si no se pudo procesar.</returns>
        public static async Task<ResponseStatus> ChangeUserDetails(string newDetails)
        {

            if (ConnectedUser == null || String.IsNullOrEmpty(ServerConnection.ConnectedUser.Token))
            {
                Console.WriteLine("ChangeUserDetails: El Token o Usuario conectado no son válidos.");
                return ResponseStatus.ACTION_FAILED;

            }

            
            var message = await SendDataAsync(ServerMessageFactory.Create(Protocol.USER, ClientUserActions.CHANGE_OTHER_DATA, [ServerConnection.ConnectedUser.Token, ConnectedUser.UserName, ConnectedUser.UserName, newDetails], indexSizeBytes: new Dictionary<int, int> { { 3,4} }));

            return ServerMessageHandler.HandleResponse(message);
        }

        #endregion
    }
}
