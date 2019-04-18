using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using WebSocket.Events;
using WebSocket.Model;
using WebSocket.Tool;

namespace WebSocket
{
    /// <summary>
    /// 服务端Listener
    /// </summary>
    public class ServerListener : EventServerBase<BaseClient>
    {
        private readonly TcpListener _listener;
        private bool _isListener;
        /// <summary>
        /// 在线客户端集合
        /// </summary>
        public readonly ConcurrentDictionary<Guid, BaseClient> Clients = new ConcurrentDictionary<Guid, BaseClient>();

        /// <summary>
        ///  实例化服务端
        /// </summary>
        public ServerListener()
            : this(10086)
        { }

        /// <summary>
        /// 实例化服务端
        /// </summary>
        /// <param name="port">端口</param>
        public ServerListener(int port)
            : this(IPAddress.Any, port)
        { }


        /// <summary>
        /// 实例化服务端
        /// </summary>
        /// <param name="ip">IP</param>
        /// <param name="port">端口</param>
        public ServerListener(string ip, int port)
            : this(IPAddress.Parse(ip), port)
        { }

        /// <summary>
        /// 实例化服务端
        /// </summary>
        /// <param name="ip">IP</param>
        /// <param name="port">端口</param>
        public ServerListener(IPAddress ip, int port)
        {
            _listener = new TcpListener(ip, port);
        }

        /// <summary>
        /// 启动监听
        /// </summary>
        public void Start()
        {
            _isListener = true;
            if (!_listener.Server.IsBound)
            {
                _listener.Start();
                try
                {
                    _listener.BeginAcceptTcpClient(Back, _listener);
                    OnLog("服务器启动监听:" + _listener.LocalEndpoint);
                }
                catch
                {
                    OnLog("服务器启动失败");
                    _isListener = false;
                }
            }
            else
            {
                OnLog("服务器已启动:" + _listener.LocalEndpoint);
            }
        }

        private void Back(IAsyncResult ar)
        {
            TcpListener lisn = (TcpListener)ar.AsyncState;

            if (!lisn.Server.IsBound)
                return;
            TcpClient ct;
            try
            {
                ct = lisn.EndAcceptTcpClient(ar);
            }
            catch
            {
                OnLog("服务器异常退出");
                return;
            }
            var stream = ct.GetStream();
            var buffer = new byte[BufferSize];
            ListenerModel model = new ListenerModel(ct, stream);
            try
            {
                stream.BeginRead(buffer, 0, BufferSize, arstream =>
                {
                    var md = (ListenerModel)arstream.AsyncState;
                    var endstream = md.NetStream;
                    var client = md.Client;
                    if (!_isListener)
                    {
                        endstream.Dispose();
                        client.Close();
                        return;
                    }
                    int len;
                    try
                    {
                        len = endstream.EndRead(arstream);
                    }
                    catch
                    {
                        OnLog(string.Format("客户端:{0} 连接失败", client.Client.RemoteEndPoint));
                        return;
                    }
                    var msg = Coding.GetString(buffer, 0, len);

                    BaseClient myclient;
                    if (msg.Contains("Sec-WebSocket-Key"))
                    {
                        //封装WebSocket握手协议
                        var clientMsg = PackageHandShakeData(buffer, len);
                        //发送握手协议
                        endstream.Write(clientMsg, 0, clientMsg.Length);

                        myclient = new ServerWebClient(client, ClientType.WebSocket);
                        PushClient(myclient);
                    }
                    else if (msg.Contains(Mark))
                    {
                        msg = msg.Remove(msg.Length - 1, 1);
                        var x = JsonHelper.Deserialize<Record>(msg);
                        if (x.Type != ComdType.Identity)
                            return;
                        var clientType = (ClientType)int.Parse(x.Data);
                        myclient = new ServerClient(client, clientType);
                        PushClient(myclient);
                    }
                    else
                    {
                        endstream.Close();
                        client.Close();
                    }
                    Array.Clear(buffer, 0, buffer.Length); // 清空缓存，避免脏读
                }, model);
            }
            catch
            {
                OnLog(string.Format("客户端:{0} 连接失败", ct.Client.RemoteEndPoint));
            }
            try
            {
                lisn.BeginAcceptTcpClient(Back, lisn);
            }
            catch
            {
                OnLog("服务器异常退出");
            }
        }

        private byte[] PackageHandShakeData(byte[] handShakeBytes, int length)
        {
            string handShakeText = Coding.GetString(handShakeBytes, 0, length);
            string key = string.Empty;
            Regex reg = new Regex(@"Sec\-WebSocket\-Key:(.*?)\r\n");
            Match m = reg.Match(handShakeText);
            if (m.Groups.Count != 0)
            {
                key = Regex.Replace(m.Value, @"Sec\-WebSocket\-Key:(.*?)\r\n", "$1").Trim();
            }

            byte[] secKeyBytes = SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(key + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"));
            string secKey = Convert.ToBase64String(secKeyBytes);

            var responseBuilder = new StringBuilder();
            responseBuilder.Append("HTTP/1.1 101 Switching Protocols\r\n");
            responseBuilder.Append("Upgrade: websocket\r\n");
            responseBuilder.Append("Connection: Upgrade\r\n");
            responseBuilder.AppendFormat("Sec-WebSocket-Accept: {0}\r\n\r\n", secKey);

            return Coding.GetBytes(responseBuilder.ToString());
        }

        private void PushClient(BaseClient myclient)
        {
            myclient.Log += myclient_Log;
            myclient.MsgRead += myclient_MsgRead;
            myclient.ClientDisconnect += myclient_ClientDisconnect;
            myclient.Id = Guid.NewGuid();
            Clients.TryAdd(myclient.Id, myclient);

            OnClientConnected(myclient);
            if (myclient.ClientType == ClientType.WebSocket)
                return;

            myclient.SendData(new Record { Type = ComdType.IdentityC, Bytes = myclient.Id.ToByteArray() });
            OnLog(string.Format("客户端:{0} 上线 客户端类型:{1}", myclient.RemoteIp, myclient.ClientType));
        }

        /// <summary>
        /// 停止监听
        /// </summary>
        public void Stop()
        {
            foreach (var c in Clients)
            {
                c.Value.Stop();
            }
            Clients.Clear();
            _listener.Stop();
            OnLog("服务器停止监听");
            _isListener = false;
        }

        /// <summary>
        /// 广播所有客户端
        /// </summary>
        /// <param name="msg">内容</param>
        /// <param name="type">客户端类型  传:null发送全部客户端</param>
        public void SendAll(Record msg, ClientType? type)
        {
            if (type == null)
                foreach (var c in Clients)
                {
                    c.Value.SendData(msg);
                }
            else
                foreach (var c in Clients.Where(m => m.Value.ClientType == type))
                {
                    c.Value.SendData(msg);
                }
        }

        //------------ 事件通知
        void myclient_MsgRead(BaseClient t, Record t2)
        {
            OnMsgRead(t, t2);
        }
        void myclient_Log(BaseClient t, string msg)
        {
            OnLog(msg);
        }
        void myclient_ClientDisconnect(BaseClient t)
        {
            BaseClient client;
            Clients.TryRemove(t.Id, out client);
            OnLog(string.Format("客户端:{0} 下线", t.RemoteIp));
            if (client == null)
                return;
            //client.Shutdown(SocketShutdown.Both);
            client.Stop();
            OnClientDisconnect(t);
        }
    }
}
