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
    public class WebHandler : Base, IHandler
    {

        private readonly StringBuilder _partialProtocal;
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public WebHandler()
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
            string json = AnalyzeClientData(bytes, length);
            return GetCommand(json);
        }

        private string AnalyzeClientData(byte[] recBytes, int length)
        {
            if (length < 2) { return string.Empty; }

            bool fin = (recBytes[0] & 0x80) == 0x80; // 1bit，1表示最后一帧  
            if (!fin)
            {
                return string.Empty;// 超过一帧暂不处理 
            }

            bool maskFlag = (recBytes[1] & 0x80) == 0x80; // 是否包含掩码  
            if (!maskFlag)
            {
                return string.Empty;// 不包含掩码的暂不处理
            }

            int payloadLen = recBytes[1] & 0x7F; // 数据长度  

            byte[] masks = new byte[4];
            byte[] payloadData;

            if (payloadLen == 126)
            {
                Array.Copy(recBytes, 4, masks, 0, 4);
                payloadLen = (UInt16)(recBytes[2] << 8 | recBytes[3]);
                payloadData = new byte[payloadLen];
                Array.Copy(recBytes, 8, payloadData, 0, payloadLen);

            }
            else if (payloadLen == 127)
            {
                Array.Copy(recBytes, 10, masks, 0, 4);
                byte[] uInt64Bytes = new byte[8];
                for (int i = 0; i < 8; i++)
                {
                    uInt64Bytes[i] = recBytes[9 - i];
                }
                UInt64 len = BitConverter.ToUInt64(uInt64Bytes, 0);

                payloadData = new byte[len];
                for (UInt64 i = 0; i < len; i++)
                {
                    payloadData[i] = recBytes[i + 14];
                }
            }
            else
            {
                Array.Copy(recBytes, 2, masks, 0, 4);
                payloadData = new byte[payloadLen];
                Array.Copy(recBytes, 6, payloadData, 0, payloadLen);
            }

            for (var i = 0; i < payloadLen; i++)
            {
                payloadData[i] = (byte)(payloadData[i] ^ masks[i % 4]);
            }

            return Coding.GetString(payloadData);
        }
    }
}
