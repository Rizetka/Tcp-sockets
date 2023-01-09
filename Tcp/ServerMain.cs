namespace Tcp
{
    class ServerMain
    {
        private int port;
        public ServerMain(int port, string serverIP)
        {
            this.port = port;
            new Server(port, serverIP);
        }
    }
}
