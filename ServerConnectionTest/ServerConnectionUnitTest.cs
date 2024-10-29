using LibraryClienteAgenda;
using System.Diagnostics.Tracing;

namespace ServerConnectionTest
{
    public class ServerConnectionUnitTest

    {
       
        #region Protocol 1: Connection/Login Responses Tests
        [Fact]//103
        public void Handle_Login_Success_Response()
        {
            
            ServerMessageFactory mFactory = new();
            ServerMessageHandler mHandler = new();
            ServerConnection connection = new(mFactory,mHandler);
         

            var user = "user";
            var message = mFactory.Create(Protocol.LOGIN, ClientLoginActions.LOGIN, ["random", "dataToCreateMessage"]);
            var token = "KKKKKKKKKKIIIIIIIIIIOOOOOOOOOOPPPPPP";
            var responseMessage = BuildResponseMessage("1", "03", [token]);
            message.DissasembleResponse(responseMessage);
          


            connection.ConnectedUser = new(user);
            ResponseStatus success = mHandler.HandleResponse(message);

            //Simulamos que hemos enviado un mensaje correcto, y recibido la respuesta que toca.
            Assert.True(success==ResponseStatus.ACTION_SUCCESS);
            Assert.Equal(token, connection.ConnectedUser.Token);
            Assert.Equal(user, connection.ConnectedUser.UserName);

        }
        [Fact]//102
        public void Handle_Logout_Success_Response()
        {
            ServerMessageFactory mFactory = new();
            ServerMessageHandler mHandler = new();
            ServerConnection connection = new(mFactory, mHandler);
           

            var message = mFactory.Create(Protocol.LOGIN, ClientLoginActions.LOGIN, ["random", "dataToCreateMessage"]);

            var token = "KKKKKKKKKKIIIIIIIIIIOOOOOOOOOOPPPPPP";
            var responseMessage = BuildResponseMessage("1", "02", [token]);
            message.DissasembleResponse(responseMessage);
            var user = "user";

            connection.ConnectedUser = new(user);
            connection.ConnectedUser.Token = token;


            ResponseStatus success = mHandler.HandleResponse(message);

            //Simulamos que hemos enviado un mensaje correcto, y recibido la respuesta que toca.
            Assert.True(success == ResponseStatus.ACTION_SUCCESS);
            Assert.Null(connection.ConnectedUser);

        }
        [Fact] //104
        public void Handle_ChangePassword_Success_Response() {
            ServerMessageFactory mFactory = new();
            ServerMessageHandler mHandler = new();
            ServerConnection connection = new(mFactory, mHandler);
           
            var token = "KKKKKKKKKKIIIIIIIIIIOOOOOOOOOOPPPPPP";
            var responseMessage = BuildResponseMessage("1", "04", [token]);
            var message = mFactory.Create(Protocol.LOGIN, ClientLoginActions.LOGIN, ["random", "dataToCreateMessage"]);
            message.DissasembleResponse(responseMessage);
            var user = "user";

            connection.ConnectedUser = new(user);
            connection.ConnectedUser.Token = token;


            var success = mHandler.HandleResponse(message);

            Assert.True(success == ResponseStatus.ACTION_SUCCESS);
            Assert.Equal(connection.ConnectedUser.Token, token);
            Assert.NotNull(connection.ConnectedUser);


        }


        #endregion

        #region Protocol 2: User Info Responses Tests
        [Fact] //215
        public void Handle_User_GetInfo_Admin_Success_Response()
        {
            ServerMessageFactory mFactory = new();
            ServerMessageHandler mHandler = new();
            ServerConnection connection = new(mFactory, mHandler);
          ;

            //Protocol (1byte)  + action (2bytes) + sizeToken(2 bytes) + Token (always 36 bytes)

            var token = "KKKKKKKKKKIIIIIIIIIIOOOOOOOOOOPPPPPP";
            var user = "user";
            var rol = "userRol";
            var realName = "realName";
            var dateBorn = "dateBorn";
            var extraData = "No hay datos extras";
            var message = mFactory.Create(Protocol.LOGIN, ClientLoginActions.LOGIN, ["random", "dataToCreateMessage"]);
           

            var responseMessage = BuildUserInfoResponseMessage("2", "15", token, user, rol, realName, dateBorn, extraData, isAdmin: true);
            message.DissasembleResponse(responseMessage);
            Console.WriteLine(responseMessage);

            //El usuario esta logeado asi que debería tener el token y su nombre asignados correctamente.
            connection.ConnectedUser = new(user);
            connection.ConnectedUser.Token = token;


            ResponseStatus success = mHandler.HandleResponse(message);
            

            //Simulamos que hemos enviado un mensaje correcto, y los datos pertinentes han cambiado.
            Assert.True(success==ResponseStatus.ACTION_SUCCESS);
            Assert.NotNull(connection.ConnectedUser);
            Assert.True(connection.ConnectedUser.IsAdmin);
            Assert.Equal(token, connection.ConnectedUser.Token);
            Assert.Equal(connection.ConnectedUser.UserName, user);
            Assert.Equal(rol, connection.ConnectedUser.Userrol);
            Assert.Equal(realName, connection.ConnectedUser.FullName);
            Assert.Equal(dateBorn, connection.ConnectedUser.DataNaixement);
            Assert.Equal(extraData, connection.ConnectedUser.DatosExtra);

        }
        [Fact(Skip = "No implementado")] //220
        public void Handle_FullName_Changed_Success_Response() { }
        [Fact(Skip = "No implementado")] //221
        public void Handle_DateBorn_Changed_Success_Response() { }
        [Fact(Skip = "No implementado")] //222
        public void Handle_OtherData_Changed_Success_Response() { }

        #endregion

        #region Protocol 9: Error Responses Tests
        #endregion

        #region Helper Methods

        private static string BuildResponseMessage(string protocol, string action, List<string> data)
        {
            //Random sentMessage
            var message = new ServerMessage<ClientLoginActions>(Protocol.LOGIN, ClientLoginActions.LOGIN, data);
            
            string response = protocol + action;
            foreach(var entry in data)
            {
                response += $"{entry.Length:D2}{entry}";

            }

            return response;
        }

        private static string BuildUserInfoResponseMessage(string protocol, string action, string token, string user,string role, string realName, string dateBorn,string extraData, bool isAdmin)
        {
            var data2BytesOffset = new List<string> { token, user, role, realName, dateBorn };
            var responseMessage = BuildResponseMessage(protocol, action, data2BytesOffset);
            var data4ByesOffset = new List<string>() { extraData };

            foreach (var entry in data4ByesOffset)
            {
                responseMessage += $"{entry.Length:D4}{entry}";
            }

            responseMessage += isAdmin ? "1" : "0";
            return responseMessage;
        }

        #endregion

    }


}