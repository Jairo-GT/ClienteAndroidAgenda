using System;
using System.Collections.Generic;
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

        public async Task<string> SendDataAsync(string dataToSend)
        {
            try
            {
              
                using TcpClient tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(ip, port);

                using NetworkStream networkStream = tcpClient.GetStream();
                byte[] data = Encoding.UTF8.GetBytes(dataToSend);
                await networkStream.WriteAsync(data, 0, data.Length);

                await networkStream.FlushAsync();

                byte[] responseBuffer = new byte[1024];

                int bytesRead = await networkStream.ReadAsync(responseBuffer, 0, responseBuffer.Length);
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
