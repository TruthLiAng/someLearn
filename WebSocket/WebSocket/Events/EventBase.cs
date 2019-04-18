using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocket.Model;

namespace WebSocket.Events
{
    /// <summary>
    /// 服务端基础事件
    /// </summary>
    public class EventServerBase<T> : Base where T : class
    {
        /// <summary>
        /// 读取完整消息
        /// </summary>
        public event Events<T, Record> MsgRead;
        /// <summary>
        /// 读取到完整消息
        /// </summary>
        /// <param name="t">基础客户端类</param>
        /// <param name="t2">命令集</param>
        protected virtual void OnMsgRead(T t, Record t2)
        {
            var handler = MsgRead;
            if (handler != null) handler(t, t2);
        }

        /// <summary>
        /// 客户端上线
        /// </summary>
        public event Events<T> ClientConnected;
        /// <summary>
        /// 客户端上线
        /// </summary>
        /// <param name="t">基础客户端类</param>
        protected virtual void OnClientConnected(T t)
        {
            var handler = ClientConnected;
            if (handler != null) handler(t);
        }

        /// <summary>
        /// 客户端断开
        /// </summary>
        public event Events<T> ClientDisconnect;
        /// <summary>
        /// 客户端断开
        /// </summary>
        /// <param name="t">基础客户端类</param>
        protected virtual void OnClientDisconnect(T t)
        {
            var handler = ClientDisconnect;
            if (handler != null) handler(t);
        }

        /// <summary>
        /// 日志
        /// </summary>
        public event Events<string> Log;

        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="client">基础客户端类</param>
        /// <param name="msg">内容</param>
        protected virtual void OnLog(string msg)
        {
            var handler = Log;
            if (handler != null)
                handler(msg);
        }
    }

    /// <summary>
    /// 客户端基础事件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventBase<T> : Base where T : class
    {

        /// <summary>
        /// 读取到完整消息
        /// </summary>
        public event Events<T, Record> MsgRead;
        /// <summary>
        /// 读取到完整消息
        /// </summary>
        /// <param name="t">基础客户端类</param>
        /// <param name="t2">命令集</param>
        protected virtual void OnMsgRead(T t, Record t2)
        {
            var handler = MsgRead;
            if (handler != null) handler(t, t2);
        }

        /// <summary>
        /// 发送完成
        /// </summary>
        public event Events<T, Record> MsgSend;
        /// <summary>
        /// 发送完成
        /// </summary>
        /// <param name="t">基础客户端类</param>
        /// <param name="t2">命令集</param>
        protected virtual void OnMsgSend(T t, Record t2)
        {
            var handler = MsgSend;
            if (handler != null) handler(t, t2);
        }

        /// <summary>
        /// 日志
        /// </summary>
        public event Events<T, string> Log;

        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="t">基础客户端类</param>
        /// <param name="msg">消息</param>
        protected virtual void OnLog(T t, string msg)
        {
            var handler = Log;
            if (handler != null)
                handler(t, msg);
        }

        /// <summary>
        /// 客户端断开
        /// </summary>
        public event Events<T> ClientDisconnect;
        /// <summary>
        /// 客户端断开
        /// </summary>
        /// <param name="t">基础客户端类</param>
        protected virtual void OnClientDisconnect(T t)
        {
            var handler = ClientDisconnect;
            if (handler != null) handler(t);
        }
        /// <summary>
        /// 客户端连成功
        /// </summary>
        public event Events<T, bool, string> ClientConnected;
        /// <summary>
        /// 客户端连状态
        /// </summary>
        /// <param name="t">基础客户端类</param>
        /// <param name="state">连接状态</param>
        /// <param name="msg">消息</param>
        protected virtual void OnClientConnected(T t, bool state, string msg)
        {
            var handler = ClientConnected;
            if (handler != null) handler(t, state, msg);
        }

    }
}
