using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Tcp
{
    static class ClientsStorage
    {
        static private List<ClientData> Clients_list = new List<ClientData>();
        static public void Add(ClientData cd)
        {
            if (check(cd))
            {
                for (int i = 0; i < Clients_list.Count; i++)
                {
                    if (Clients_list[i].clientIP == cd.clientIP)
                    {
                        Clients_list[i].UnicCode = cd.UnicCode;
                    }
                }
            }
            else
            {
                Clients_list.Add(cd);
            }
        }
        static public string getClientIpByCode(string uc)
        {
            string ip = null;

            for (int i = 0; i < Clients_list.Count; i++)
            {
                if (Clients_list[i].UnicCode == uc)
                {
                    ip = Clients_list[i].clientIP;
                }
            }
            return ip;
        }
        static public Socket getClientSocketByIP(string ClientIP)
        {
            Socket sc = null;

            for (int i = 0; i < Clients_list.Count; i++)
            {
                if (Clients_list[i].clientIP == ClientIP)
                {
                    sc = Clients_list[i].availableSocket;
                }
            }
            //Console.WriteLine($"{ (((IPEndPoint)sc.LocalEndPoint).Address.ToString()) } <---> { (((IPEndPoint)sc.RemoteEndPoint).Address.ToString()) }");
            return sc;
        }
        static public void ShowClients()
        {
            for (int i = 0; i < Clients_list.Count; i++)
            {
                Console.WriteLine($"{ Clients_list[i].clientIP } | { Clients_list[i].UnicCode } | { (((IPEndPoint)Clients_list[i].availableSocket.LocalEndPoint).Address.ToString()) } <---> { (((IPEndPoint)Clients_list[i].availableSocket.RemoteEndPoint).Address.ToString()) }");
            }
        }
        static private bool check(ClientData cd)
        {
            for (int i = 0; i < Clients_list.Count; i++)
            {
                if (Clients_list[i].clientIP == cd.clientIP)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
