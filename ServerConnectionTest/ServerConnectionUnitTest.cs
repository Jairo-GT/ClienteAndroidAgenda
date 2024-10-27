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


            //Protocol (1byte)  + action (2bytes) + sizeToken(2 bytes) + Token (siempre 36 bytes)
            var token = "KKKKKKKKKKIIIIIIIIIIOOOOOOOOOOPPPPPP";
            var responseMessage = BuildResponseMessage("1", "03", [token]);
            var user = "user";

            ServerConnection.ConnectedUser = new(user);
            ResponseStatus success = ServerConnection.HandleResponse(responseMessage);

            //Simulamos que hemos enviado un mensaje correcto, y recibido la respuesta que toca.
            Assert.True(success==ResponseStatus.ACTION_SUCCESS);
            Assert.Equal(token, ServerConnection.ConnectedUser.Token);
            Assert.Equal(user, ServerConnection.ConnectedUser.UserName);

        }
        [Fact]//102
        public void Handle_Logout_Success_Response()
        {


            //Protocol (1byte)  + action (2bytes) + sizeToken(2 bytes) + Token (always 36 bytes)
            var token = "KKKKKKKKKKIIIIIIIIIIOOOOOOOOOOPPPPPP";
            var responseMessage = BuildResponseMessage("1", "02", [token]);
            var user = "user";

            ServerConnection.ConnectedUser = new(user);
            ServerConnection.ConnectedUser.Token = token;


            ResponseStatus success = ServerConnection.HandleResponse(responseMessage);

            //Simulamos que hemos enviado un mensaje correcto, y recibido la respuesta que toca.
            Assert.True(success == ResponseStatus.ACTION_SUCCESS);
            Assert.Null(ServerConnection.ConnectedUser);

        }
        [Fact] //104
        public void Handle_ChangePassword_Success_Response() {
            
            var token = "KKKKKKKKKKIIIIIIIIIIOOOOOOOOOOPPPPPP";
            var responseMessage = BuildResponseMessage("1", "04", [token]);
            var user = "user";

            ServerConnection.ConnectedUser = new(user);
            ServerConnection.ConnectedUser.Token = token;


            var success = ServerConnection.HandleResponse(responseMessage);

            Assert.True(success == ResponseStatus.ACTION_SUCCESS);
            Assert.Equal(ServerConnection.ConnectedUser.Token, token);
            Assert.NotNull(ServerConnection.ConnectedUser);


        }


        #endregion

        #region Protocol 2: User Info Responses Tests
        [Fact] //215
        public void Handle_User_GetInfo_Admin_Success_Response()
        {


            //Protocol (1byte)  + action (2bytes) + sizeToken(2 bytes) + Token (always 36 bytes)

            var token = "KKKKKKKKKKIIIIIIIIIIOOOOOOOOOOPPPPPP";
            var user = "user";
            var rol = "userRol";
            var realName = "realName";
            var dateBorn = "dateBorn";
            var extraData = "No hay datos extras";

            var responseMessage = BuildUserInfoResponseMessage("2", "15", token, user, rol, realName, dateBorn, extraData, isAdmin: true);
            Console.WriteLine(responseMessage);

            //El usuario esta logeado asi que debería tener el token y su nombre asignados correctamente.
            ServerConnection.ConnectedUser = new(user);
            ServerConnection.ConnectedUser.Token = token;


            ResponseStatus success = ServerConnection.HandleResponse(responseMessage);
            

            //Simulamos que hemos enviado un mensaje correcto, y los datos pertinentes han cambiado.
            Assert.True(success==ResponseStatus.ACTION_SUCCESS);
            Assert.NotNull(ServerConnection.ConnectedUser);
            Assert.True(ServerConnection.ConnectedUser.IsAdmin);
            Assert.Equal(token, ServerConnection.ConnectedUser.Token);
            Assert.Equal(ServerConnection.ConnectedUser.UserName, user);
            Assert.Equal(rol, ServerConnection.ConnectedUser.Userrol);
            Assert.Equal(realName, ServerConnection.ConnectedUser.FullName);
            Assert.Equal(dateBorn, ServerConnection.ConnectedUser.DataNaixement);
            Assert.Equal(extraData, ServerConnection.ConnectedUser.DatosExtra);

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