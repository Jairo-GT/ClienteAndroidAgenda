using LibraryClienteAgenda;
using System.Diagnostics.Tracing;

namespace ServerConnectionTest
{
    public class ServerConnectionUnitTest

    {
        [Fact]
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


        [Fact]
        public void Handle_Logout_Responsee()
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




        [Fact]//Solo dato de admin de momento, Expandir con los otros datos después.
        public void Handle_User_GetInfo_Admin_Response()
        {


            //Protocol (1byte)  + action (2bytes) + sizeToken(2 bytes) + Token (always 36 bytes)
            var protocol = "2";
            var action = "15";

            var token = "KKKKKKKKKKIIIIIIIIIIOOOOOOOOOOPPPPPP";

            List<string> data = [token, "user", "user2", "userRol", "realName", "dateBorn"];

            var responseMessage = protocol + action;
            foreach (var entry in data)
            {

                responseMessage += string.Format("{0:D2}", entry.Length) + entry;
            }
            Console.WriteLine(responseMessage);

            responseMessage += "1";  //Es admin
            var user = "user";

            ServerConnection.ConnectedUser = new(user);

            ServerConnection.Token = token;


            bool success = ServerConnection.HandleResponse(responseMessage);

            //Simulamos que hemos enviado un mensaje correcto, y recibido la respuesta que toca.
            Assert.True(success);
            Assert.True(ServerConnection.ConnectedUser.IsAdmin);
            Assert.Equal(token, ServerConnection.Token);
            Assert.Equal(ServerConnection.ConnectedUser.UserName, user);
            //TODO: Comprobar otros datos


        }

    }
}