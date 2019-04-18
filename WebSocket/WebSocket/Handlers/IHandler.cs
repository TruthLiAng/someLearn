using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocket.Model;

namespace WebSocket.Handlers
{
    /// <summary>
    /// 处理函数接口
    /// </summary>
    public interface IHandler
    {
        /// <summary>
        /// 转为命令对象
        /// </summary>
        /// <param name="input">命令字符串</param>
        /// <returns>命令集合</returns>
        IEnumerable<Record> GetCommand(string input);

        /// <summary>
        /// 转为命令对象
        /// </summary>
        /// <param name="bytes">命令</param>
        /// <param name="length">长度</param>
        /// <returns>命令集合</returns>
        IEnumerable<Record> GetCommand(byte[] bytes, int length);
    }
}
