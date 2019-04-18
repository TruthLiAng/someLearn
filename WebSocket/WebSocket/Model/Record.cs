using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocket.Model
{
    /// <summary>
    /// 数据封装
    /// </summary>
    [Serializable]
    public struct Record
    {
        /* /// <summary>
        /// 默认构造函数
        /// </summary>
        public Record()
        {
            Bytes = new byte[0];
        }*/

        /// <summary>
        /// 命令类型
        /// </summary>
        public ComdType Type { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        [JsonIgnore]
        public string Data
        {
            get { return Encoding.UTF8.GetString(Bytes); }
        }

        /// <summary>
        /// byte数据
        /// </summary>
        public byte[] Bytes { get; set; }
    }
}
