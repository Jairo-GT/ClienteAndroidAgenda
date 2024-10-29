using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryClienteAgenda
{
    public class ServerMessageHandler : IServerMessageHandler
    {
        private IServerConnection servI;

        public ServerMessageHandler() { }

        public void SetConnection(IServerConnection serverInterface)
        {
            servI = serverInterface;

        }
        /// <summary>
        /// Devuelve un diccionario con los datos parseados de una respuesta de servidor.
        /// </summary>
        /// <param name="keys">Las keys del diccionario devuelto despues de extraer los datos. Los extrae de manera secuencial.</param>
        /// <param name="data">Los datos a extraer formados por un offset que marca el tamaño y el dato en si en una String.</param>
        /// <param name="offsetSize">Tamaño del offset en bytes antes de cada campo sucesivo.</param>
        /// <param name="offsetSize">Diccionario con los datos segregados, si es null creara uno nuevo si los agregara al diccionario y lo devolverá.</param>
        /// <returns>Un diccionario con los campos indicados extraidos de la string data. Si hay alguna excepción devolverá un diccionario vacío.</returns>
        public Dictionary<string, string> ParseData(List<string> keys, string data, int offsetSize = 2, Dictionary<string, string>? parsedData = null)
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
            catch (Exception)
            {
                parsedData = null;
                return parsedData;

            }



        }


        /// <summary>
        /// Procesa una respuesta de servidor.
        /// </summary>
        /// <param name="response">La respuesta del servidor.</param>
        /// <returns>Devuelve un ResponseStatus con SUCCES,FAILED,ERROR segun si la acción tuvo éxito,falló o devolvió un mensaje de error/denegación. </returns>
        public ResponseStatus HandleResponse<TAction>(ServerMessage<TAction> message) where TAction : Enum
        {

            var parsedAction = message.ResponseActionIntValue;
            var data = message.ResponseMessageData;

            switch ((Protocol)message.ResponseProtocolIntValue)
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
        private ResponseStatus HandleServerErrorActions(ServerErrorActions serverAction, string data)
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
        private ResponseStatus HandleServerUserActions(ServerUserActions serverAction, string data)
        {

            switch (serverAction)
            {

                case ServerUserActions.USER_INFO:
                    Console.WriteLine("Handling USER INFO");
                    var userInfoData = ParseData(["token", "user", "userRol", "realName", "dateBorn"], data);

                    if (userInfoData != null && servI.ConnectedUser != null)
                    {
                        servI.ConnectedUser.IsAdmin = data.Trim().LastOrDefault().ToString() == "1";
                        servI.ConnectedUser.FullName = userInfoData["realName"];
                        servI.ConnectedUser.DataNaixement = userInfoData["dateBorn"];
                        servI.ConnectedUser.Userrol = userInfoData["userRol"];
                        servI.ConnectedUser.Token = userInfoData["token"];
                        var parsedData = ParseData(["extraData"], userInfoData["nonParsedData"], 4, userInfoData);
                        servI.ConnectedUser.DatosExtra = parsedData != null && parsedData.TryGetValue("extraData", out var extraData) ? extraData : "";
                    }
                    else return ResponseStatus.ACTION_FAILED;

                    return ResponseStatus.ACTION_SUCCESS;
                case ServerUserActions.CANVI_NOM:
                    Console.WriteLine("Handling CHANGE_FULLNAME");
                    var ChangeFullnameData = ParseData(["token", "user", "userRol", "realName", "dateBorn"], data);
                    if (ChangeFullnameData != null && servI.ConnectedUser != null)
                    {
                        if (servI.ConnectedUser.Token == ChangeFullnameData["token"])
                            return ResponseStatus.ACTION_SUCCESS;
                    }
                    break;
                case ServerUserActions.CANVI_DATA:
                    Console.WriteLine("Handling CHANGE_DATE_BORN");
                    var ChangeDateData = ParseData(["token", "user", "userRol", "realName", "dateBorn"], data);
                    if (ChangeDateData != null && servI.ConnectedUser != null)
                    {
                        if (servI.ConnectedUser.Token == ChangeDateData["token"])
                            return ResponseStatus.ACTION_SUCCESS;
                    }
                    break;
                case ServerUserActions.CANVI_ALTRES:
                    Console.WriteLine("Handling CHANGE_DETAILS");
                    var ChangeDetailsData = ParseData(["token", "user", "userRol", "realName", "dateBorn"], data);
                    if (ChangeDetailsData != null && servI.ConnectedUser != null)
                    {
                        if (servI.ConnectedUser.Token == ChangeDetailsData["token"])
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
        private ResponseStatus HandleServerLoginActions(ServerLoginActions serverAction, string data)
        {



            switch (serverAction)
            {
                case ServerLoginActions.LOGIN_SUCCESS:
                    Console.WriteLine("Handling LOGIN_SUCCESS");
                    var retTokenLogin = ParseData(["token"], data);
                    if (servI.ConnectedUser != null && retTokenLogin != null && retTokenLogin.TryGetValue("token", out string? valueTokenLogin))
                    {

                        System.Console.WriteLine("TOKEN OBTAINED: " + valueTokenLogin);
                        servI.ConnectedUser.Token = valueTokenLogin;
                        return ResponseStatus.ACTION_SUCCESS;
                    }

                    break;
                case ServerLoginActions.LOGOUT_SUCCESS:
                    Console.WriteLine("Handling LOGOUT_SUCCESS");
                    var retTokenLogout = ParseData(["token"], data);
                    if (servI.ConnectedUser != null && retTokenLogout != null && retTokenLogout.TryGetValue("token", out string? valueTokenLogout))
                    {
                        Console.WriteLine($"TokenInResponse:{valueTokenLogout}");
                        Console.WriteLine($"TokenCurrentlyActive:{servI.ConnectedUser.Token}");

                        if (servI.ConnectedUser != null && servI.ConnectedUser.Token != null && valueTokenLogout == servI.ConnectedUser.Token)
                        {
                            servI.ConnectedUser.Token = "";
                            servI.ConnectedUser = null;
                            return ResponseStatus.ACTION_SUCCESS;
                        }

                    }
                    break;
                case ServerLoginActions.RESET_PASSWORD_AFTER_LOGOUT:
                    Console.WriteLine("Handling CHANGE_PASSWORD");
                    var retTokenChangePassword = ParseData(["token"], data);
                    if (servI.ConnectedUser != null && retTokenChangePassword != null && retTokenChangePassword.TryGetValue("token", out string? valueTokenChangePassword))
                    {

                        if (servI.ConnectedUser.Token != null && valueTokenChangePassword == servI.ConnectedUser.Token)
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





    }
}
