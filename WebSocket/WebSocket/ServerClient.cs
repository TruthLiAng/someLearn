using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WebSocket.Handlers;
using WebSocket.Model;

namespace WebSocket
{
    /// <summary>
    /// 服务器使用
    /// </summary>
    public class ServerClient : BaseClient
    {
        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="c">Client</param>
        /// <param name="type">客户端类型</param>
        public ServerClient(TcpClient c, ClientType type)
            : base(type)
        {
            Client = c;
            // 获得流
            Stream = Client.GetStream();
            Buffer = new byte[BufferSize];
            Handler = new JsonHandler();

            RemoteIp = c.Client.RemoteEndPoint;
            LocalIp = c.Client.LocalEndPoint;

            //开始准备读取
            Stream.BeginRead(Buffer, 0, BufferSize, ReadComplete, Stream);
        }
    }
}
