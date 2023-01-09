using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

//cd Desktop\Tcp\Tcp\bin\Debug>Tcp.exe 
namespace Tcp
{
    class Server
    {
        private int port;
        private string serverIP;
        private Socket listenSocket;
        private IPEndPoint ipServerPoint;
        private bool isRunning = false;
        private Thread startTransmission;

        public Server(int port, string serverIP)
        {
            this.port = port;
            this.serverIP = serverIP;
            //Dns.GetHostEntry(Dns.GetHostName()).AddressList[2].ToString();
            Console.WriteLine($"IP servera: {serverIP}");

            ipServerPoint = new IPEndPoint(IPAddress.Parse(serverIP), port);
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.ReceiveBufferSize = 256;

            Console.WriteLine($"Cервер запущен на порте: {port} c IP {serverIP}");
            Console.WriteLine("Ожидание подключений...");
            Console.WriteLine("================================================");

            listening();
        }
        private void listening()
        {
            isRunning = true;

            listenSocket.Bind(ipServerPoint);
            listenSocket.Listen(10);

            int bytes = 0;
            byte[] commandFromClient = new byte[128];

            while (isRunning)
            {
                Socket handler = listenSocket.Accept();
                StringBuilder packetMsg = new StringBuilder();

                for (int i = 0; i < 1; i++)
                {
                    bytes = handler.Receive(commandFromClient);
                    packetMsg.Append(Encoding.Unicode.GetString(commandFromClient, 0, bytes));
                    i++;
                }

                FirstPacketsHandler packHand = new FirstPacketsHandler(Convert.ToString(packetMsg), handler);
                startTransmission = new Thread(packHand.processFirstPacket);
                startTransmission.Start();
            }
        }
    }
}
