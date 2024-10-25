using LibraryClienteAgenda;
using System.Diagnostics.Tracing;

namespace ServerConnectionTest
{
    public class ServerConnectionUnitTest

    {

        //RESPUESTA DEL PROTOCOLO 1 CONEXIÓN/LOGIN MOCK
        [Fact]//103
        public void Handle_Login_Success_Response()
        {


            //Protocol (1byte)  + action (2bytes) + sizeToken(2 bytes) + Token (always 36 bytes)
            var protocol = "1";
            var action = "03";
            var token = "KKKKKKKKKKIIIIIIIIIIOOOOOOOOOOPPPPPP";
            var responseMessage = protocol + action + token.Length + token;
            var user = "user";

            ServerConnection.ConnectedUser = new(user);
            bool success = ServerConnection.HandleResponse(responseMessage);

            //Simulamos que hemos enviado un mensaje correcto, y recibido la respuesta que toca.
            Assert.True(success);
            Assert.Equal(token, ServerConnection.Token);
            Assert.Equal(user, ServerConnection.ConnectedUser.UserName);

        }
        [Fact]//102
        public void Handle_Logout_Response()
        {


            //Protocol (1byte)  + action (2bytes) + sizeToken(2 bytes) + Token (always 36 bytes)
            var protocol = "1";
            var action = "02";
            var token = "KKKKKKKKKKIIIIIIIIIIOOOOOOOOOOPPPPPP";
            var responseMessage = protocol + action + token.Length + token;
            var user = "user";

            ServerConnection.ConnectedUser = new(user);
            ServerConnection.Token = token;


            bool success = ServerConnection.HandleResponse(responseMessage);

            //Simulamos que hemos enviado un mensaje correcto, y recibido la respuesta que toca.
            Assert.True(success);
            Assert.Empty(ServerConnection.Token);
            Assert.Null(ServerConnection.ConnectedUser);

        }
        [Fact]//104
        public void Handle_ChangePassword_After_Logout_Response() { }
    
        //RESPUESTA DEL PROTOCOLO 2 USUARIO MOCK
        [Fact] //215
        public void Handle_User_GetInfo_Admin_Response()
        {


            //Protocol (1byte)  + action (2bytes) + sizeToken(2 bytes) + Token (always 36 bytes)
            var protocol = "2";
            var action = "15";

            var token = "KKKKKKKKKKIIIIIIIIIIOOOOOOOOOOPPPPPP";
            var user = "user";
            var user2 = "user2";
            var rol = "userRol";
            var realName = "realName";
            var dateBorn = "dateBorn";

            List<string> data = [token, user, user2, "userRol", "realName", "dateBorn"];

            var responseMessage = protocol + action;
            foreach (var entry in data)
            {

                responseMessage += string.Format("{0:D2}", entry.Length) + entry;
            }
            Console.WriteLine(responseMessage);

            responseMessage += "1";  //Es admin
            

            ServerConnection.ConnectedUser = new(user);

            ServerConnection.Token = token;


            bool success = ServerConnection.HandleResponse(responseMessage);
            //TODO: Saber cual es el user principal 1 o 2 y hacer comprobación también.  Y faltaria el campo extra de 4 bytes pero no tengo muy claro aún para que es.
            //Simulamos que hemos enviado un mensaje correcto, y recibido la respuesta que toca.
            Assert.True(success);
            Assert.True(ServerConnection.ConnectedUser.IsAdmin);
            Assert.Equal(token, ServerConnection.Token);
            Assert.Equal(ServerConnection.ConnectedUser.UserName, user);
            Assert.Equal(rol, ServerConnection.ConnectedUser.Userrol);
            Assert.Equal(realName, ServerConnection.ConnectedUser.FullName);
            Assert.Equal(dateBorn, ServerConnection.ConnectedUser.DataNaixement);
           

        }
        [Fact]
        public void Handle__Response() { }
    }
}