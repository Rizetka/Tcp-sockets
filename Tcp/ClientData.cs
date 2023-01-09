using System;
using System.Net.Sockets;
namespace Tcp
{
    class ClientData
    {
        public string clientIP { get; set; }
        public string UnicCode { get; set; }
        public Socket availableSocket { get; set; }

        public ClientData(string clientIP, Socket availableSocket)
        {
            this.clientIP = clientIP;
            this.availableSocket = availableSocket;
            genCode();
        }

        public void genCode()
        {
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] unCode = new char[32];

            Random rand = new Random();

            for (int i = 0; i < 32; i++)
            {
                unCode[i] = alphabet[rand.Next(0, 62)];
            }

            UnicCode = new string(unCode);
        }
    }
}
