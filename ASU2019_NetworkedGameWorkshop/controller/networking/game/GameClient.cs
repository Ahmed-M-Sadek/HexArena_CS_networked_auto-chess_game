using System.Net.Sockets;

namespace ASU2019_NetworkedGameWorkshop.controller.networking.game
{
    class GameClient : GameNetworkManager
    {
        private readonly string hostname;
        private readonly int port;

        public GameClient(string hostname, int port)
        {
            this.hostname = hostname;
            this.port = port;
        }

        public override void start()
        {
            startThreads(new TcpClient(hostname, port));
        }
    }
}
