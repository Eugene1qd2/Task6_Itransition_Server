using OnlinePaintServer.Models;
using System.Net;
using System.Net.Sockets;

ServerObject server = new ServerObject();
server.boardsData.Add(new BoardData("hehe", "Board for everyone"));
await server.ListenAsync();