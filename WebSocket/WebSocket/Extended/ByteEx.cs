using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocket.Extended
{
    public static class ByteEx
    {
        /// <summary>
        /// string转byte数组
        /// </summary>
        public static byte[] GetBytes(this string c)
        {
            return c == null ? null : Base.Coding.GetBytes(c);
        }

        /// <summary>
        /// int转byte数组
        /// </summary>
        public static byte[] GetBytes(this int c)
        {
            return Base.Coding.GetBytes(c.ToString());
        }
        /// <summary>
        /// char数组转byte数组
        /// </summary>
        public static byte[] GetBytes(this char[] c)
        {
            return c == null ? null : Base.Coding.GetBytes(c);
        }
    }
}
