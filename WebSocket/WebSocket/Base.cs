using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WebSocket.Handlers;

namespace WebSocket
{
    /// <summary>
    /// 基础类
    /// </summary>
    public abstract class Base
    {
        /// <summary>
        /// 字符编码
        /// </summary>
        public static readonly Encoding Coding = Encoding.UTF8;
        protected const int BufferSize = 8192;
        protected string Mark = "⊙";
        protected byte[] Buffer;

        protected IHandler Handler;
        protected NetworkStream Stream;

        protected TcpClient Client;
    }
}
