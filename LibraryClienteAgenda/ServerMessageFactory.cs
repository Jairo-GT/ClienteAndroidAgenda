using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryClienteAgenda
{
    public class ServerMessageFactory
    {
        public ServerMessage<TAction> Create<TAction>(Protocol requestProtocol, TAction requestAction, List<string> data, Dictionary<int,int>? indexSizeBytes = null) where TAction :Enum
        {
            return new ServerMessage<TAction>(requestProtocol, requestAction, data,indexSizeBytes);
        }
    }
}
