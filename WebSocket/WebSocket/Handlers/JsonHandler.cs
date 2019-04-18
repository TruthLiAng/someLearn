using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocket.Model;
using WebSocket.Tool;

namespace WebSocket.Handlers
{
    /// <summary>
    /// 协议处理程序
    /// </summary>
    public class JsonHandler : Base, IHandler
    {
        private readonly StringBuilder _partialProtocal;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public JsonHandler()
        {
            _partialProtocal = new StringBuilder();
        }

        private string[] GetProtocol(List<string> outputList = null)
        {
            if (outputList == null)
                outputList = new List<string>();

            string input = _partialProtocal.ToString();
            int removelength = input.LastIndexOf(Mark, StringComparison.Ordinal);
            if (removelength > 0)
            {
                input = input.Substring(0, removelength);
                string[] datas = input.Split(new[] { Mark }, StringSplitOptions.RemoveEmptyEntries);
                outputList.AddRange(datas);
                _partialProtocal.Remove(0, removelength + 1);
            }
            return outputList.ToArray();
        }

        /// <summary>
        /// 转为命令对象
        /// </summary>
        /// <param name="input">命令字符串</param>
        /// <returns>命令集合</returns>
        public IEnumerable<Record> GetCommand(string input)
        {
            _partialProtocal.Append(input);
            var strs = GetProtocol();
            if (_partialProtocal.Length > 8192000)
                _partialProtocal.Remove(0, 4096000);
            return strs.Select(JsonHelper.Deserialize<Record>);
        }

        public IEnumerable<Record> GetCommand(byte[] bytes, int length)
        {
            return GetCommand(Coding.GetString(bytes, 0, length));
        }
    }
}
