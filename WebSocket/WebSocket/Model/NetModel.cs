using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WebSocket.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class NetModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="netStream"></param>
        /// <param name="record"></param>
        public NetModel(NetworkStream netStream, Record record)
        {
            NetStream = netStream;
            Record = record;
        }
        /// <summary>
        /// 
        /// </summary>
        public NetworkStream NetStream { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Record Record { get; set; }
    }
}
