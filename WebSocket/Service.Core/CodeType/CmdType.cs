using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Core.CodeType
{
    /// <summary>
    /// socket发送/接收消息类
    /// </summary>
    public class MessageInfo
    {
        public CmdType Type { get; set; }
        public string Data { get; set; }
    }

    public enum CmdType
    {
        /// <summary>
        /// socket首次连接
        /// </summary>
        Connect = 10000,

        /// <summary>
        /// 传输消息格式错误
        /// </summary>
        Error = 00000,

        /// <summary>
        /// 用户信息
        /// </summary>
        UserInfo = 10001,

        /// <summary>
        /// 设置是否中奖
        /// </summary>
        RedEnvInfo = 20001,

        /// <summary>
        /// 获取中奖人数
        /// </summary>
        GetRedInfo = 20002,

        /// <summary>
        /// 获取是否有抽奖资格
        /// </summary>
        IsJoined = 20003,
    }
}