using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace PruebaMauiAndroid.Models
{
    class ServerConnection
    {
        private static string ip;
        private static int port;
        public static string token;
        public static UserInfo user;


        public static async Task<bool> userLogout()
        {



            if (!String.IsNullOrEmpty(ServerConnection.token))
            {

                string dataToSend = "205" + string.Format("{0:D2}", ServerConnection.token.Length) + ServerConnection.token +
                      string.Format("{0:D2}", user.userName.Length) + user.userName;


                string response = await SendDataAsync(dataToSend);


                return true;
               //TODO: Confirmar que la respuesta es el mismo token  y borrar token actual.
            
            }


                return false;
        }

     

        public ServerConnection(string ip, int port) {

            ServerConnection.ip = ip;
            ServerConnection.port = port;
        
        
        }

       
        public void ExtractAndSetToken(string response)
        {

            if(!String.IsNullOrEmpty(response) && response.Length>2)
            {
                string offset = response.Substring(5, 2);
                int offsetInt = 0;
                if (int.TryParse(offset, out offsetInt) && offsetInt > 0)
                {

                    token = response.Substring(7,36);
                }

            }
            

        }

        //Envia data y espera timeoutMilliseconds (5 segons de default) per rebre una resposta del server o cancela el socket
        public static async Task<string> SendDataAsync(string dataToSend,int timeoutMilliseconds = 5000)
        {
            try
            {
              
                using TcpClient tcpClient = new TcpClient()
                {
                    NoDelay = true 
                };



                using var cts = new CancellationTokenSource(timeoutMilliseconds);

                Task connectTask = tcpClient.ConnectAsync(ip, port);
                if (await Task.WhenAny(connectTask, Task.Delay(timeoutMilliseconds, cts.Token)) != connectTask)
                {
                    throw new TimeoutException("Connection timed out.");
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
