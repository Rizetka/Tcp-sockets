using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Tcp
{
    class FileTransHandler
    {
        private Socket Client_ReciverSocket, Client_SenderSocket;
        private bool isActive = false;
        public FileTransHandler(Socket Client_ReciverSocket, string client_senderIP)
        {
            Client_SenderSocket = ClientsStorage.getClientSocketByIP(client_senderIP);
            this.Client_ReciverSocket = Client_ReciverSocket;
            isActive = true;

        }
        public void processTrans()// от клиента1 к клиенту2 файл
        {
            byte[] startMsg = new byte[256]; // буфер для ответа, для списка папок рабочего стола к клиенту с сокетом Client_ReciverSocket
            int bytes = 0; // количество полученных байт
            StringBuilder formFoldList;

            StringBuilder getListComm = new StringBuilder();

            startMsg = Encoding.Unicode.GetBytes("/ok/");
            Client_ReciverSocket.ReceiveBufferSize = 8192;
            Client_SenderSocket.ReceiveBufferSize = 8192;
            Client_ReciverSocket.Send(startMsg);

            do
            {
                bytes = Client_ReciverSocket.Receive(startMsg);
                getListComm.Append(Encoding.Unicode.GetString(startMsg, 0, bytes));
            }
            while (Client_ReciverSocket.Available > 0);

            Console.WriteLine($"{ getListComm.ToString() } Клиент с IP {(((IPEndPoint)Client_ReciverSocket.RemoteEndPoint).Address.ToString())}: Просит список папок рабочего стола клиента с IP {(((IPEndPoint)Client_SenderSocket.RemoteEndPoint).Address.ToString())}");

            if (getListComm.ToString() == "/gl/")
            {
                Client_SenderSocket.Send(Encoding.Unicode.GetBytes("/gl/"));

                //formFoldList = new List<byte>();
                formFoldList = new StringBuilder();
                do
                {
                    bytes = Client_SenderSocket.Receive(startMsg, startMsg.Length, 0);
                    formFoldList.Append(Encoding.Unicode.GetString(startMsg, 0, bytes));
                }
                while (Client_SenderSocket.Available > 0);

                Client_ReciverSocket.Send(Encoding.Unicode.GetBytes(formFoldList.ToString())); // передаем форматированный дист папок раб. стола

                while (isActive == true) // работает пока соединения не разорвано. Разрыв соединения тогда, когда получатель отказывается от пересылки файла либо все файлы отправлены. Код прерывания  - /br/
                {
                    byte[] bytesCmnd = new byte[512];
                    StringBuilder cmndFromReciver = new StringBuilder();

                    do
                    {
                        bytes = Client_ReciverSocket.Receive(bytesCmnd, bytesCmnd.Length, 0);
                        cmndFromReciver.Append(Encoding.Unicode.GetString(bytesCmnd, 0, bytes));
                    }
                    while (Client_ReciverSocket.Available > 0);

                    if (cmndFromReciver.ToString().StartsWith("/br/"))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Клиент с IP: { ((IPEndPoint)Client_ReciverSocket.RemoteEndPoint).Address.ToString() } передумал принимать файл от клиента с IP { ((IPEndPoint)Client_SenderSocket.RemoteEndPoint).Address.ToString() }");

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"    Разрыв соединения: { ((IPEndPoint)Client_SenderSocket.RemoteEndPoint).Address.ToString() } ==X==> { ((IPEndPoint)Client_ReciverSocket.RemoteEndPoint).Address.ToString() }");
                        Console.ResetColor();

                        Client_SenderSocket.Send(Encoding.Unicode.GetBytes("/br/"));

                        Client_ReciverSocket.Shutdown(SocketShutdown.Both);
                        //Client_SenderSocket.Shutdown(SocketShutdown.Both);
                        Client_ReciverSocket.Close();
                        //Client_SenderSocket.Close();

                        isActive = false;
                    }
                    if (cmndFromReciver.ToString().StartsWith("/bk/"))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Клиент с IP: { ((IPEndPoint)Client_ReciverSocket.RemoteEndPoint).Address.ToString() } просит список содержимого предыдущей папки клиента с IP { ((IPEndPoint)Client_SenderSocket.RemoteEndPoint).Address.ToString() }");
                        Console.ResetColor();

                        Client_SenderSocket.Send(Encoding.Unicode.GetBytes("/bk/"));

                        byte[] bytesForList = new byte[256];

                        formFoldList = new StringBuilder();

                        do
                        {
                            bytes = Client_SenderSocket.Receive(bytesForList, bytesForList.Length, 0); // тут возможно будтт прикол 
                            formFoldList.Append(Encoding.Unicode.GetString(bytesForList, 0, bytes));
                        }
                        while (Client_SenderSocket.Available > 0);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Клиент с IP: { ((IPEndPoint)Client_SenderSocket.RemoteEndPoint).Address.ToString() } передает список содержимого предыдущей папки клиенту с IP { ((IPEndPoint)Client_ReciverSocket.RemoteEndPoint).Address.ToString() }");
                        Console.ResetColor();

                        Client_ReciverSocket.Send(Encoding.Unicode.GetBytes(formFoldList.ToString())); // передаем форматированный лист содержимого папки или отказ

                    }
                    if (cmndFromReciver.ToString().StartsWith("/go/"))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Клиент с IP: { ((IPEndPoint)Client_ReciverSocket.RemoteEndPoint).Address.ToString() } переходит на следующую папку клиента с IP { ((IPEndPoint)Client_SenderSocket.RemoteEndPoint).Address.ToString() }");
                        Console.ResetColor();

                        Client_SenderSocket.Send(Encoding.Unicode.GetBytes(cmndFromReciver.ToString()));

                        byte[] bytesForNextList = new byte[256];

                        formFoldList = new StringBuilder();

                        do
                        {
                            bytes = Client_SenderSocket.Receive(bytesForNextList, bytesForNextList.Length, 0); // тут возможно будтт прикол 
                            formFoldList.Append(Encoding.Unicode.GetString(bytesForNextList, 0, bytes));
                        }
                        while (Client_SenderSocket.Available > 0);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Клиент с IP: { ((IPEndPoint)Client_ReciverSocket.RemoteEndPoint).Address.ToString() } получает содержимое следующей папки клиента с IP { ((IPEndPoint)Client_SenderSocket.RemoteEndPoint).Address.ToString() }");
                        Console.ResetColor();

                        Client_ReciverSocket.Send(Encoding.Unicode.GetBytes(formFoldList.ToString()));
                    }
                    if (cmndFromReciver.ToString().StartsWith("/ld/"))
                    {
                        Client_SenderSocket.Send(Encoding.Unicode.GetBytes(cmndFromReciver.ToString()));

                        Client_ReciverSocket.ReceiveBufferSize = 128;
                        Client_SenderSocket.ReceiveBufferSize = 128;
                       
                        byte[] checkBytes = new byte[16];

                        do
                        {
                            Client_SenderSocket.Receive(checkBytes);
                            Console.WriteLine(Encoding.Unicode.GetString(checkBytes));
                        }
                        while (Client_SenderSocket.Available > 0);

                        if (Encoding.Unicode.GetString(checkBytes).StartsWith("/st/"))
                        {
                            Client_ReciverSocket.Send(checkBytes); //st

                            while(isActive)
                            { 
                                byte[] nxBytes = new byte[8];

                                Client_ReciverSocket.Receive(nxBytes);

                                if (Encoding.Unicode.GetString(nxBytes) == "/nx/")
                                {
                                    Client_SenderSocket.Send(Encoding.Unicode.GetBytes("/nx/"));

                                    byte[] trafficBytes = new byte[128];

                                    Client_SenderSocket.Receive(trafficBytes);

                                    Client_ReciverSocket.Send(trafficBytes); Console.WriteLine($"Получатель с IP: {(((IPEndPoint)Client_ReciverSocket.RemoteEndPoint).Address.ToString())} получает 128 байт: { Encoding.Unicode.GetString(trafficBytes) }");
                                }
                            }
                        }
                        else
                        {
                            Client_ReciverSocket.Send(checkBytes);
                        }
                    }
                }
            }
        }
    }
}
