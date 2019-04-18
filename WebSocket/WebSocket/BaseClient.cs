using System;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using WebSocket.Events;
using WebSocket.Model;
using WebSocket.Tool;
using Service.Core;
using WebSocket.Extended;

namespace WebSocket
{
    /// <summary>
    /// 基础客户端
    /// </summary>
    public abstract class BaseClient : EventBase<BaseClient>
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 远程网络地址
        /// </summary>
        public EndPoint RemoteIp { get; set; }
        /// <summary>
        /// 本地网络地址
        /// </summary>
        public EndPoint LocalIp { get; set; }

        /// <summary>
        /// 客户端类型
        /// </summary>
        public ClientType ClientType { get; private set; }

        /// <summary>
        /// 客户端对象是否为空
        /// </summary>
        public bool ClientIsNull
        {
            get { return Client == null; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">客户端类型</param>
        protected BaseClient(ClientType type)
        {
            ClientType = type;
        }

        /// <summary>
        /// 停止
        /// </summary>
        public virtual void Stop()
        {
            if (Stream != null)
            {
                lock (Stream)
                {
                    Stream.Close();
                    Stream = null;
                }
            }
            if (Client == null)
                return;

            Client.Close();
            Client = null;
        }

        #region 发送内容
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="comm">命令结构</param>
        public virtual void SendData(Record comm)
        {
            if (comm.Bytes == null)
                comm.Bytes = new byte[0];
            string msg = JsonHelper.Serialize(comm);
            msg = string.Format("{0}{1}", msg, Mark);
            byte[] temp = Coding.GetBytes(msg); // 获得缓存
            Send(temp, comm);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="bts">发送内容</param>
        /// <param name="comm">命令结构</param>
        protected void Send(byte[] bts, Record comm)
        {
            Thread.Sleep(1);
            if (Client == null)
                return;
            if (Client.IsOnlineRead())
            {
                try
                {
                    lock (Stream)
                    {
                        Stream.BeginWrite(bts, 0, bts.Length, WriteComplete, new NetModel(Stream, comm));
                    }
                }
                catch (Exception ex)
                {
                    Loghelper.Error(ex);
                }
                Loghelper.Debug(string.Format("发送命令:{0} 数据长度:{1}", comm.Type, bts.Length));
            }
            else
            {
                OnClientDisconnect(this);
            }
        }

        private void WriteComplete(IAsyncResult ar)
        {
            var netmodel = (NetModel)ar.AsyncState;
            try
            {
                netmodel.NetStream.EndWrite(ar);
            }
            catch (Exception ex)
            {
                Loghelper.Error(ex);
            }
            OnMsgSend(this, netmodel.Record);
        }

        #endregion

        #region 读取内容

        /// <summary>
        /// 读取发送内容
        /// </summary>
        /// <param name="ar">异步操作</param>
        protected virtual void ReadComplete(IAsyncResult ar)
        {
            try
            {
                var stream = (NetworkStream)ar.AsyncState;
                if (Client.IsOnlineRead())
                {
                    var bytesRead = stream.EndRead(ar);
                    Loghelper.Debug(string.Format("读取到 {0} 字节 ...", bytesRead));
                    if (bytesRead == 0)
                        return;

                    string msg = Coding.GetString(Buffer, 0, bytesRead);
                    Array.Clear(Buffer, 0, Buffer.Length); // 清空缓存，避免脏读

                    var comms = Handler.GetCommand(msg);
                    foreach (Record comm in comms)
                    {
                        ParameterizedThreadStart start = HandleProtocol;
                        start.BeginInvoke(comm, null, null);
                    }
                    // 再次调用BeginRead()，完成时调用自身，形成无限循环
                    stream.BeginRead(Buffer, 0, BufferSize, ReadComplete, stream);
                }
                else
                {
                    OnClientDisconnect(this);
                }
            }
            catch (Exception e)
            {
                Loghelper.Error(e);
            }
        }

        /// <summary>
        /// 命令相关执行内容
        /// </summary>
        /// <param name="obj"></param>
        protected void HandleProtocol(object obj)
        {
            Record comm = (Record)obj;
            switch (comm.Type)
            {
                case ComdType.Identity:
                    ClientType = (ClientType)int.Parse(comm.Data);
                    break;
                case ComdType.IdentityC:
                    Id = new Guid(comm.Bytes);
                    OnClientConnected(this, true, "连接成功");
                    break;
                default:
                    Loghelper.Debug(string.Format("执行{0}命令", comm.Type));
                    OnMsgRead(this, comm);
                    break;
            }
        }
        #endregion
    }
}
