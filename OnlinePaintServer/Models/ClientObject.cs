using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OnlinePaintServer.Models
{
    public class ClientObject
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        public string currentBoardId {  get; set; }
        public StreamWriter Writer { get; }
        public StreamReader Reader { get; }
        public Stream Stream { get; }

        public TcpClient client;
        public ServerObject server;

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            client = tcpClient;
            server = serverObject;
            Stream = client.GetStream();
            Reader = new StreamReader(Stream);
            Writer = new StreamWriter(Stream);
        }
        protected internal void Close()
        {
            Writer.Close();
            Reader.Close();
            client.Close();
        }
    }
}
