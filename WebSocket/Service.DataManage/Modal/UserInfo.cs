using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DataManage.Modal
{
    /// <summary>
    /// 用户信息和中奖信息类
    /// </summary>
    public class UserInfo
    {
        public string Name { get; set; }
        public string Id { get; set; }

        public DateTime startTime { get; set; }

        /// <summary>
        /// 0：未中奖；1：中奖
        /// </summary>
        public int ISLuck { get; set; } = 0;

        /// <summary>
        /// 上次参与抽奖时间
        /// </summary>
        public DateTime LuckTime { get; set; }
    }
}