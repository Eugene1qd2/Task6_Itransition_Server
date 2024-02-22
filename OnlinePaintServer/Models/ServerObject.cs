using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using OnlinePaintServer.Models.Serealize;

namespace OnlinePaintServer.Models
{
    public class ServerObject
    {
        TcpListener tcpListener = new TcpListener(IPAddress.Any, 8888);
        List<ClientObject> clients = new List<ClientObject>();
        public List<BoardData> boardsData = new List<BoardData>();

        private string GetElementaryBoardsData()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("*boards*");
            foreach (var board in boardsData)
            {
                sb.Append(board.ToString() + "|");
            }
            return sb.ToString();
        }

        private string GetBoardData(string boardId)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"*{boardId}*");
            var currentBoard = boardsData.First(x => x.boardId == boardId);
            if (currentBoard != null)
            {
                foreach (var line in currentBoard.lines)
                {
                    sb.Append(line.ToString() + "|");
                }
            }
            return sb.ToString();
        }

        protected internal void RemoveConnection(string id)
        {
            ClientObject? client = clients.FirstOrDefault(c => c.Id == id);
            if (client != null) clients.Remove(client);
            client?.Close();
        }

        protected internal async Task ListenAsync()
        {
            try
            {
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    clients.Add(clientObject);
                    await Console.Out.WriteLineAsync(clientObject.Id + " connected");
                    Task.Run(async () =>
                    {
                        await SendBoardsDataAsync(clientObject.Id);
                        while (clientObject.client.Connected)
                        {
                            try
                            {
                                var line = clientObject.Reader.ReadLine();
                                if (line != null)
                                {
                                    Console.WriteLine(line);
                                    var command = line.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);
                                    switch (command[0])
                                    {
                                        case "append":
                                            await ExecAppendCommandAsync(command[1], clientObject.Id); //готово
                                            break;
                                        case "get":
                                            clientObject.currentBoardId= command[1];
                                            await SendBoardDataAsync(clientObject.Id, command[1]); //готово
                                            break;
                                        case "create":
                                            await CreateNewBoard(command[1], clientObject.Id); //готово
                                            break;
                                        case "clear":
                                            await ClearBoard(command[1]);
                                            break;
                                        default:
                                            Console.WriteLine("cant recognize command:\n"+line);
                                            break;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                                clientObject.Close();
                                clients.Remove(clientObject);
                            }
                        }
                    });

                    //туда
                    //1. *boards*id досок+имя через|        при подключении без вариантов
                    //2. *id доски*инфа о линиях через|     по запросу
                    //3. *add*инфа о лини                   по запросу
                    //3. *clear*id доски                    по запросу

                    //сюда
                    //1. *get*id доски -> 2.
                    //2. *append*id доски|инфа о линии -> 3.
                    //3. *create*id доски+имя доски -> 2.
                    //4. *clear*id доски -> 2.

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }

        private async Task ClearBoard(string id)
        {
            boardsData.First(x => x.boardId == id).lines.Clear();
            foreach (var client in clients)
            {
                if (client.currentBoardId == id)
                {
                    await BroadcastMessageAsync(client.Id, $"*clear*{id}");
                }
            }
        }

        private async Task CreateNewBoard(string boardData, string senderId)
        {
            var data = boardData.Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
            BoardData board = new BoardData(data[0], data[1]);
            boardsData.Add(board);
            SendBoardDataAsync(senderId, board.boardId);
        }


        //append
        private async Task NotifyUsersData(string boardId, LineObject line, string senderId)
        {
            foreach (var client in clients)
            {
                if (client.Id != senderId)
                    if (client.currentBoardId == boardId)
                    {
                        await client.Writer.WriteLineAsync("*add*" + line.ToString());
                        await client.Writer.FlushAsync();
                        await Console.Out.WriteLineAsync("*add*" + line.ToString());
                    }
            }
        }

        private async Task ExecAppendCommandAsync(string command, string senderId)
        {
            var parts = command.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            string boardId = parts[0];
            LineObject line = LineObject.FromString(parts[1]);
            await NotifyUsersData(boardId, line, senderId);
            AppendBoardWithLine(boardId, line);
        }

        private void AppendBoardWithLine(string boardId, LineObject line)
        {
            boardsData.First(x => x.boardId == boardId).AppendLine(line);
        }
        //append-end

        //get

        private async Task SendBoardsDataAsync(string distId)
        {
            await BroadcastMessageAsync(distId, GetElementaryBoardsData());
        }

        private async Task SendBoardDataAsync(string distId, string boardId)
        {
            await BroadcastMessageAsync(distId, GetBoardData(boardId));
        }

        protected internal async Task BroadcastMessageAsync(string id, string message)
        {
            var destination = clients.First(x => x.Id == id);
            if (destination != null)
            {
                await destination.Writer.WriteLineAsync(message);
                await destination.Writer.FlushAsync();
            }
        }
        //get-end

        //disconnect
        protected internal void Disconnect()
        {
            foreach (var client in clients)
            {
                client.Close();
            }
            tcpListener.Stop();
        }
    }
}
