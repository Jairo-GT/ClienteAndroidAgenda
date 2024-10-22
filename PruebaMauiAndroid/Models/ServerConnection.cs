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
        private string ip;
        private int port;
        private string token;
       

        public async Task<string> populateUserInfo(UserInfo user) {


            


            if (!String.IsNullOrEmpty(this.token))
            {


                string dataToSend = "205"+string.Format("{0:D2}",this.token.Length)+this.token+ 
                    string.Format("{0:D2}", user.userName.Length)+user.userName +
                    string.Format("{0:D2}", user.userName.Length) + user.userName;


                Debug.WriteLine(dataToSend);
                string response = await SendDataAsync(dataToSend);


                return response;

               



            }


            return "false";        
        
        }

        public ServerConnection(string ip, int port) {

            this.ip = ip;
            this.port = port;
        
        
        }

        
        public void ExtractAndSetToken(string response)
        {

            if(!String.IsNullOrEmpty(response) && response.Length>2)
            {
                string offset = response.Substring(0, 2);
                int offsetInt = 0;
                if (int.TryParse(offset, out offsetInt) && offsetInt > 0)
                {

                    token = response.Substring(2);
                }

            }
            

        }

        public async Task<string> SendDataAsync(string dataToSend,int timeoutMilliseconds = 5000)
        {
            try
            {
              
                using TcpClient tcpClient = new TcpClient();
              


                using var cts = new CancellationTokenSource(timeoutMilliseconds);

                Task connectTask = tcpClient.ConnectAsync(ip, port);
                if (await Task.WhenAny(connectTask, Task.Delay(timeoutMilliseconds, cts.Token)) != connectTask)
                {
                    throw new TimeoutException("Connection timed out.");
                }

                using NetworkStream networkStream = tcpClient.GetStream();
                networkStream.WriteTimeout = timeoutMilliseconds;
                networkStream.ReadTimeout = timeoutMilliseconds;


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
