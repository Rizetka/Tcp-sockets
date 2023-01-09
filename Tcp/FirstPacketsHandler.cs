using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Tcp
{
    class FirstPacketsHandler
    {
        private string packetContent;
        private Socket transSocket;
        public FirstPacketsHandler(string packetContent, Socket transHandler)
        {
            this.packetContent = packetContent;
            transSocket = transHandler;
        }
        public void processFirstPacket()
        {
            ClientData client = new ClientData((((IPEndPoint)transSocket.RemoteEndPoint).Address.ToString()), transSocket);
            byte[] data = new byte[128];

            string csh = packetContent.Substring(0, 4);

            switch (csh)
            {
                case "/gc/":
                    ClientsStorage.Add(client);
                    Console.WriteLine("------------------------------------------------");
                    Console.WriteLine($"Клиент с IP {client.clientIP}: Просит генерацию уникального ключа");

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"    Ключ: {client.UnicCode}");
                    Console.ResetColor();

                    transSocket.Send(Encoding.Unicode.GetBytes(client.UnicCode));

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Клиент с IP {client.clientIP}: Сокет запомнен, ожидает передачи файла...");
                    Console.ResetColor();

                    //ClientsStorage.ShowClients();
                    break;
                case "/gf/":
                    string code = packetContent.Remove(0, 4);

                    if (client.clientIP == ClientsStorage.getClientIpByCode(code))
                    {
                        Console.WriteLine("------------------------------------------------");
                        Console.WriteLine($"Клиент с IP {client.clientIP}: Просит передачу файла от {client.clientIP}");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"    Передача самому себе. Передача отклонена");
                        Console.ResetColor();

                        data = Encoding.Unicode.GetBytes("/fl/Соединение разорвано. Попытка передачи файла самому себе");

                        transSocket.Send(data);
                        transSocket.Shutdown(SocketShutdown.Both);
                        transSocket.Close();

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Клиент с IP {client.clientIP}: Разрыв соединения");
                        Console.ResetColor();

                    }
                    else if (ClientsStorage.getClientIpByCode(code) != null)
                    {
                        if (client.clientIP != ClientsStorage.getClientIpByCode(code))
                        {
                            string client_senderIP = ClientsStorage.getClientIpByCode(code);

                            Console.WriteLine("------------------------------------------------");
                            Console.WriteLine($"Клиент с IP {client.clientIP}: Просит передачу файла от {client_senderIP} \n    Передача начата");

                            FileTransHandler fileTrHand = new FileTransHandler(transSocket, client_senderIP);
                            Thread transProcces = new Thread(fileTrHand.processTrans);
                            transProcces.Start();
                        }
                    }
                    else
                    {
                        data = Encoding.Unicode.GetBytes("/fl/Такой ссылки нет");

                        transSocket.Send(data);
                        transSocket.Shutdown(SocketShutdown.Both);
                        transSocket.Close();

                        Console.WriteLine("-------------------------------------");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Клиент с IP {client.clientIP} ввел несуществующую ссылку");
                        Console.ResetColor();

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Клиент с IP {client.clientIP}: Разрыв соединения");
                        Console.ResetColor();
                    }
                    break;
                default:
                    Console.WriteLine("-------------------------------------");
                    Console.WriteLine($"Неизвестное содержвние пакета: {packetContent}");
                    transSocket.Shutdown(SocketShutdown.Both);
                    transSocket.Close();
                    break;
            }
        }
    }
}
