using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClientTcp
{
    class Client
    {
        private string serverIP;
        private int serverPort = 8005;
        private IPEndPoint ipPoint;
        //private Socket clientSocket;
        public Client(string serverIP)
        {
            this.serverIP = serverIP;
            ipPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);

            Console.WriteLine($"Произошло подключение к серверу с IP {serverIP}");
            Console.WriteLine("================================================");
        }
        public void HandleCommand(string command)
        {
            byte[] data = new byte[256]; // буфер для команды серверу
            StringBuilder builder = new StringBuilder();

            Socket clientSocket;

            switch (command.Substring(0, 4))
            {
                case "/gc/":
                    clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    clientSocket.Connect(ipPoint);

                    data = Encoding.Unicode.GetBytes("/gc/");
                    clientSocket.Send(data);

                    FileHandler fileSendHand = new FileHandler(clientSocket);
                    fileSendHand.startSendingFile();
                    //Thread transSendProcces = new Thread(fileSendHand.startSendingFile);
                    //transSendProcces.Start();
                    break;
                case "/gf/":
                    clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    clientSocket.Connect(ipPoint);

                    data = Encoding.Unicode.GetBytes(command);
                    clientSocket.Send(data);

                    FileHandler fileResHand = new FileHandler(clientSocket);
                    fileResHand.startReceivingFile();
                    //Thread fileResTrHand = new Thread(fileResHand.startReceivingFile);
                    //fileResTrHand.Start();
                    break;
                default:
                    Console.WriteLine("Такой команды нет...");
                    break;
            }
        }
    }
}
