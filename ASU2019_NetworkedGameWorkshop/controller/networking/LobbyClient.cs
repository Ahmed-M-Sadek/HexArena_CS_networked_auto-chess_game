using ASU2019_NetworkedGameWorkshop.view;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ASU2019_NetworkedGameWorkshop.controller.networking
{
    class LobbyClient
    {
        private readonly string ip;
        private readonly int port;
        private readonly ConnectForm connectForm;

        public LobbyClient(string ip, int port, ConnectForm connectForm)
        {
            this.ip = ip;
            this.port = port;
            this.connectForm = connectForm;
        }

        public void connectToServer()
        {
            Thread thread = new Thread(() => handleServer())
            {
                IsBackground = true
            };
            thread.Start();
        }

        private void handleServer()
        {
            using (TcpClient tcpClient = new TcpClient(ip, port))
            using (StreamReader streamReader = new StreamReader(tcpClient.GetStream()))
            using (StreamWriter streamWriter = new StreamWriter(tcpClient.GetStream()))
            {
                streamWriter.WriteLine("LOBBY");
                streamWriter.Flush();

                string clientIP = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString();
                clientIP = clientIP.Substring(clientIP.LastIndexOf(':') + 1);
                connectForm.LobbyMembers.Enqueue($"{clientIP}\t(HOST)");

                while (true)
                {
                    string[] msg = streamReader.ReadLine().Split('#');
                    if (msg[0].Equals("MEMBER"))
                    {
                        connectForm.LobbyMembers.Enqueue(msg[1]);
                    }
                }
            }
        }
    }
}
