using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocket.Model
{
    /// <summary>
    /// 命令类型(发送接收是服务器角度)
    /// 命令:5开头服务器用 1开头地图用 2开头web用
    /// 命令解析:5001 5服务器用 0发送 0功能 1功能对应操作
    ///          1111 1地图用 1接收 1功能 1功能对应操作
    ///          20111 2web用 0发送 1功能 1功能对应操作 1子功能
    /// </summary>
    public enum ComdType
    {
        #region 服务器  5
        /// <summary>
        /// 心跳包
        /// </summary>
        Heartbeat = 5001,

        /// <summary>
        /// 身份(客户端->服务端)
        /// </summary>
        Identity = 5002,
        /// <summary>
        /// 身份(服务端->客户端)
        /// </summary>
        IdentityC = 5003,
        #endregion

        #region web用 2

        #region websocket通讯
        /// <summary>
        /// 全网网站首页连接
        /// </summary>
        WebsocketHomeConnect = 21999,
        WebsocketBaseDataChanged = 21998,
        /// <summary>
        /// 向网站转发指纹信息
        /// </summary>
        WebSendMessage = 20666,
        /// <summary>
        /// 向网站发送将要到期的人员日志
        /// </summary>
        WebLogMessage = 20777,
        /// <summary>
        /// 转发刷新信息
        /// </summary>
        WebRefresh =20888,
        #endregion

        #region 网络状态检测 4001 
        /// <summary>
        /// 网络状态检测
        /// </summary>
        networkCheck = 4001,
        /// <summary>
        /// 检测结果发送
        /// </summary>
        networkCheckStop = 4011,
        #endregion
        #endregion

    }
}
