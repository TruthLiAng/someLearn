using Service.Core.CodeType;
using Service.Core.Interface;
using SuperSocket.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Socket.SocketInterface
{
    public interface ISocket: IDependency
    {
        void Send(WebSocketSession session, CmdType type, string msg);

        void SendAll(string msg, CmdType type);
    }
}
