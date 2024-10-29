using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryClienteAgenda
{

    //TODO: Control de errores, valores nulos etc
    public class ServerMessage<TAction> where TAction : Enum
    {
      

        public ResponseStatus ResponseStatus { get; private set; }
        public string RequestProtocol { get; private set; }
        public string RequestAction { get; private set; }
        public string RequestMessage { get { return RequestProtocol + RequestAction + RequestMessageData; } }
        public string RequestMessageData { get; private set; } = "";

        public string ResponseProtocol { get; private set; }
        public string ResponseAction { get; private set; }
        public string ResponseMessage { get { return ResponseProtocol + ResponseAction + ResponseMessageData; } }
        public string ResponseMessageData { get; private set; } = "";
        public int ResponseProtocolIntValue { get {

                if(int.TryParse(ResponseProtocol, out int parsedProtocol))
                {
                    return parsedProtocol;

                }
                return -1;
            } }
        public int ResponseActionIntValue { get
            {

                if (int.TryParse(ResponseAction, out int parsedAction))
                {
                    return parsedAction;

                }
                return -1;
            }
        }

        public string ErrorMessage { get; set; }

        public ServerMessage(Protocol requestProtocol,TAction requestAction,List<string> data, Dictionary<int, int>? byteSizes = null) 
        {
            if (requestAction == null || requestAction == null || data == null) throw new Exception("ServerMessage: Fields requestProtocol,requestAction,data cannot be null");

            RequestProtocol = Convert.ToInt32(requestProtocol).ToString();
            RequestAction = Convert.ToInt32(requestAction).ToString("D2");
            AssembleData(data, byteSizes);
        }

        public void DissasembleResponse(string response)
        {

            string data;

            _ = int.TryParse(response[..1], out int parsedProtocol);

            int parsedAction;
            //Los otros son siempre 2 bytes para la acción pero en el 9 puede haber más (4 creo segun la wiki).
            if (parsedProtocol != 9)
            {


                 _ = int.TryParse(response.AsSpan(1, 2), out parsedAction);
                data = response[3..];
                ResponseAction = parsedAction.ToString("D2");
            }
            else
            {

                _ = int.TryParse(response.AsSpan(1, 4), out parsedAction);
                data = response[5..];
                ResponseAction = parsedAction.ToString("D4");

            }

            ResponseProtocol = parsedProtocol.ToString(); ;
            ResponseMessageData = data;

            Console.WriteLine($"RESPONSE PROTOCOL: {parsedProtocol}");
            Console.WriteLine($"RESPONSE ACTION: {parsedAction}");
            Console.WriteLine($"RESPONSE DATA: {data}");



        }
        private void AssembleData(List<string> data, Dictionary<int, int>? byteSizes)
        {
            int defaultBytes = 2;

            for (int i = 0; i < data.Count; i++)
            {
                string entry = data[i];
                int prefixSize = byteSizes != null && byteSizes.TryGetValue(i, out int value) ? value : defaultBytes;

                string lengthPrefix = entry.Length.ToString().PadLeft(prefixSize, '0');



                RequestMessageData += lengthPrefix + entry;
            }


        }
    }
}



