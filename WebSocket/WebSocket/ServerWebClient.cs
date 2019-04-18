using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocket.Extended;
using WebSocket.Handlers;
using WebSocket.Model;
using WebSocket.Tool;

namespace WebSocket
{
    /// <summary>
    /// 服务器使用
    /// </summary>
    internal class ServerWebClient : BaseClient
    {
        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="c">Client</param>
        /// <param name="type">客户端类型</param>
        public ServerWebClient(TcpClient c, ClientType type)
            : base(type)
        {
            Client = c;
            // 获得流
            Stream = Client.GetStream();
            Buffer = new byte[BufferSize];
            Handler = new WebHandler();

            RemoteIp = c.Client.RemoteEndPoint;
            LocalIp = c.Client.LocalEndPoint;

            //开始准备读取
            try
            {
                Stream.BeginRead(Buffer, 0, BufferSize, ReadComplete, Stream);
            }
            catch
            {
                OnLog(this, string.Format("客户端:{0} 连接失败", c.Client.RemoteEndPoint));
            }
        }

        public override void SendData(Record comm)
        {
            string msg = JsonHelper.Serialize(comm);
            PackageSend(msg, comm);
        }

        /// <summary>
        /// WebSocket Send 发送数据到客户端,打包服务器数据
        /// </summary>
        /// <param name="msg">消息内容</param>
        /// <param name="comm">命令结构</param>
        private void PackageSend(string msg, Record comm)
        {
            byte[] bytes = Coding.GetBytes(msg);

            bool canSend = true;
            //每次最大发送 64Kb的数据
            var SendMax = 65536;

            int num = 0;
            //已经发送的字节数据
            int taked = 0;

            var datas = new List<byte>();
            while (canSend)
            {
                //内容数据
                byte[] contentBytes = null;
                var sendArr = bytes.Skip(num * SendMax).Take(SendMax).ToArray();
                taked += sendArr.Length;
                if (sendArr.Length > 0)
                {
                    //是否可以继续发送
                    canSend = bytes.Length > taked;
                    if (sendArr.Length < 126)
                    {
                        #region 一次发送小于126的数据

                        contentBytes = new byte[sendArr.Length + 2];
                        contentBytes[0] = (byte)(num == 0 ? 0x81 : (!canSend ? 0x80 : 0));
                        contentBytes[1] = (byte)sendArr.Length;
                        Array.Copy(sendArr, 0, contentBytes, 2, sendArr.Length);
                        canSend = false;

                        #endregion
                    }
                    else if (sendArr.Length < 0xFFFF)
                    {
                        #region 发送小于65535的数据

                        contentBytes = new byte[sendArr.Length + 4];
                        //首次不分片发送,大于128字节的数据一次发完
                        if (!canSend && num == 0)
                        {
                            contentBytes[0] = 0x81;
                        }
                        else
                        {
                            //一个分片的消息由起始帧（FIN为0，opcode非0），若干（0个或多个）帧（FIN为0，opcode为0），结束帧（FIN为1，opcode为0）。
                            contentBytes[0] = (byte)(num == 0 ? 0x01 : (!canSend ? 0x80 : 0));
                        }
                        contentBytes[1] = 126;
                        byte[] ushortlen = BitConverter.GetBytes((short)sendArr.Length);
                        contentBytes[2] = ushortlen[1];
                        contentBytes[3] = ushortlen[0];
                        Array.Copy(sendArr, 0, contentBytes, 4, sendArr.Length);

                        #endregion
                    }
                    else if (sendArr.LongLength < long.MaxValue)
                    {
                        #region 一次发送所有数据
                        //long数据一次发完
                        contentBytes = new byte[sendArr.Length + 10];
                        //首次不分片发送,大于128字节的数据一次发完
                        if (!canSend && num == 0)
                        {
                            contentBytes[0] = 0x81;
                        }
                        else
                        {
                            //一个分片的消息由起始帧（FIN为0，opcode非0），若干（0个或多个）帧（FIN为0，opcode为0），结束帧（FIN为1，opcode为0）。
                            contentBytes[0] = (byte)(num == 0 ? 0x01 : (!canSend ? 0x80 : 0));
                        }
                        contentBytes[1] = 127;
                        byte[] ulonglen = BitConverter.GetBytes((long)sendArr.Length);
                        contentBytes[2] = ulonglen[7];
                        contentBytes[3] = ulonglen[6];
                        contentBytes[4] = ulonglen[5];
                        contentBytes[5] = ulonglen[4];
                        contentBytes[6] = ulonglen[3];
                        contentBytes[7] = ulonglen[2];
                        contentBytes[8] = ulonglen[1];
                        contentBytes[9] = ulonglen[0];

                        Array.Copy(sendArr, 0, contentBytes, 10, sendArr.Length);

                        #endregion
                    }
                }

                if (contentBytes != null)
                {
                    datas.AddRange(contentBytes);
                }
                num++;
            }
            Send(datas.ToArray(), comm);
        }
        protected override void ReadComplete(IAsyncResult ar)
        {
            try
            {
                var stream = (NetworkStream)ar.AsyncState;
                if (Client.IsOnlineRead())
                {
                    var length = stream.EndRead(ar);
                    OnLog(this, string.Format("读取到 {0} 字节 ...", length));
                    if (length < 2)
                        return;
                    if (length == 8)
                    {
                        Stop();
                        OnClientDisconnect(this);
                        return;
                    }

                    var comms = Handler.GetCommand(Buffer, length);
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
                OnLog(this, e.Message);
            }
        }
    }
}
