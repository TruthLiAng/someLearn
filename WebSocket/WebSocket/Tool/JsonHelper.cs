using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocket.Tool
{
    /// <summary>
    /// json序列化,反序列化
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// json序列化
        /// </summary>
        /// <param name="t">对象Type</param>
        /// <typeparam name="T">对象实例</typeparam>
        /// <returns></returns>
        public static string Serialize<T>(T t)
        {
            var json = Task.Factory.StartNew(() => JsonConvert.SerializeObject(t));
            return json.Result;
        }

        /// <summary>
        /// json反序列化
        /// </summary>
        /// <typeparam name="T">对象Type</typeparam>
        /// <param name="json">json字符串</param>
        /// <returns></returns>
        public static T Deserialize<T>(string json)
        {
            var obj = Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(json));
            return obj.Result;
        }
    }
}
