using System.Net.Sockets;

namespace ASU2019_NetworkedGameWorkshop.controller.networking.game
{
    class GameServer : GameNetworkManager
    {
        private readonly int port;

        public GameServer(int port)
        {
            this.port = port;
        }

        public override void start()
        {
            TcpListener tcpListener = TcpListener.Create(port);
            tcpListener.Start();

            TcpClient tcpClient = tcpListener.AcceptTcpClient();
            startThreads(tcpClient);
        }
    }
}
