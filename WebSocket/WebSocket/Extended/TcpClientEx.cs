using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WebSocket.Extended
{
    public static class TcpClientEx
    {
        public static bool IsOnlineWrite(this TcpClient c)
        {
            return !((c.Client.Poll(100, SelectMode.SelectWrite) && (c.Client.Available == 0)) || !c.Client.Connected);
        }
        /// <summary>
        /// 连起情况判断
        /// </summary>
        /// <param name="c">client</param>
        /// <returns>false 断开连接 true连接正常</returns>
        public static bool IsOnlineRead(this TcpClient c)
        {
            if (c == null)
                return false;
            try
            {
                return
                    !((c.Client.Poll(100, SelectMode.SelectRead) && (c.Client.Available == 0)) || !c.Client.Connected);
            }
            catch
            {
                return false;
            }
        }
    }
}
