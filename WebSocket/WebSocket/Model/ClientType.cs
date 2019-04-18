using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocket.Model
{
    /// <summary>
    /// 客户端类型
    /// </summary>
    public enum ClientType
    {
        /// <summary>
        /// 无
        /// </summary>
        Null = -1,

        /// <summary>
        /// 地图
        /// </summary>
        Map = 1,

        /// <summary>
        /// WebSocket
        /// </summary>
        WebSocket = 2
    }
}
