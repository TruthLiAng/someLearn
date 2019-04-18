using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WebSocket.Model
{
    internal class ListenerModel
    {
        public ListenerModel(TcpClient client, NetworkStream stream)
        {
            Client = client;
            NetStream = stream;
        }
        public NetworkStream NetStream { get; set; }
        public TcpClient Client { get; set; }
    }
}
