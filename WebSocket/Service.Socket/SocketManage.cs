using Service.Socket.SocketInterface;
using SuperSocket.SocketBase;
using SuperSocket.WebSocket;
using SuperSocket.SocketEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Service.Core.CodeType;
using Service.DataManage.Function;

namespace Service.Socket
{
    public class SocketManage : ISocket
    {
        /// <summary>
        /// 用户集合字典
        /// </summary>
        public Dictionary<string, WebSocketSession> users = new Dictionary<string, WebSocketSession>();

        private RedDataManage dataManage = new RedDataManage();

        /// <summary>
        /// socket初始化
        /// </summary>
        public void Init()
        {
            WebSocketServer ws = new WebSocketServer();
            ws.NewMessageReceived += Ws_NewMessageReceived;//当有信息传入时
            ws.NewSessionConnected += Ws_NewSessionConnected;//当有用户连入时
            ws.SessionClosed += Ws_SessionClosed;//当有用户退出时
            //TODO 将端口等写入config中。
            if (ws.Setup(10086))//绑定端口
                ws.Start();//启动服务
        }

        private void Ws_SessionClosed(WebSocketSession session, CloseReason value)
        {
            if (users.ContainsKey(session.SessionID))
            {
                users.Remove(session.SessionID);
            }
        }

        private void Ws_NewSessionConnected(WebSocketSession session)
        {
            if (!users.ContainsKey(session.SessionID))
            {
                users.Add(session.SessionID, session);
            }
        }

        private void Ws_NewMessageReceived(WebSocketSession session, string value)
        {
            try
            {
                var dataInfo = JsonConvert.DeserializeObject<MessageInfo>(value);
                switch (dataInfo.Type)
                {
                    case CmdType.UserInfo:
                        dataManage.SaveUser(dataInfo.Data);
                        break;

                    case CmdType.RedEnvInfo:
                        dataManage.UpdateLuck(dataInfo.Data);
                        break;

                    case CmdType.GetRedInfo:
                        var res = dataManage.GetLuck();
                        Send(session, CmdType.GetRedInfo, res);
                        break;

                    case CmdType.IsJoined:
                        var msg = dataManage.HasJoined(dataInfo.Data);
                        Send(session, CmdType.IsJoined, msg);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception)
            {
                Send(session, CmdType.Error, "消息格式错误");
            }
        }

        public void Send(WebSocketSession session, CmdType type, string msg)
        {
            MessageInfo message = new MessageInfo()
            {
                Type = type,
                Data = msg
            };

            session.Send(JsonConvert.SerializeObject(message));
        }

        public void SendAll(string msg, CmdType type)
        {
            MessageInfo message = new MessageInfo()
            {
                Type = type,
                Data = msg
            };

            var msgStr = JsonConvert.SerializeObject(message);

            foreach (var session in users.Values)
            {
                if (session.Connected)
                {
                    session.Send(msgStr);
                }
            }
        }
    }
}