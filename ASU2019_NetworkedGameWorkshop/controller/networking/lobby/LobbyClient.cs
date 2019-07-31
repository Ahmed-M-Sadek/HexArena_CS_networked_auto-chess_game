using ASU2019_NetworkedGameWorkshop.view;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ASU2019_NetworkedGameWorkshop.controller.networking.lobby
{
    class LobbyClient
    {
        private readonly string ip;
        private readonly int port;
        private readonly ConnectForm connectForm;
        private readonly string playerName;
        private readonly List<Thread> threads;

        private TcpClient tcpClient;
        private bool terminated;

        public LobbyClient(string ip, int port, ConnectForm connectForm, string playerName)
        {
            this.ip = ip;
            this.port = port;
            this.connectForm = connectForm;
            this.playerName = playerName;
            terminated = false;
            threads = new List<Thread>();
        }

        public void connectToServer()
        {
            Thread thread = new Thread(() => handleServer())
            {
                IsBackground = true
            };
            threads.Add(thread);
            thread.Start();
        }

        private void handleServer()
        {
            using (tcpClient = new TcpClient(ip, port))
            using (StreamReader streamReader = new StreamReader(tcpClient.GetStream()))
            using (StreamWriter streamWriter = new StreamWriter(tcpClient.GetStream()))
            {
                streamWriter.WriteLine("LOBBY");
                streamWriter.WriteLine(playerName);
                streamWriter.Flush();

                string clientIP = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString();
                clientIP = clientIP.Substring(clientIP.LastIndexOf(':') + 1);
                connectForm.LobbyMembers.Enqueue($"{streamReader.ReadLine()}\t({clientIP})\t(HOST)");

                while (!terminated)
                {
                    string fullMsg = streamReader.ReadLine();
                    if (fullMsg != null)
                    {
                        string[] msg = fullMsg.Split(game.GameNetworkManager.NETWORK_MSG_SEPARATOR);
                        if (msg[0].Equals("MEMBER"))
                        {
                            connectForm.LobbyMembers.Enqueue($"{msg[2]}\t({msg[1]})");
                        }
                        else if (msg[0].Equals("STARTGAME"))
                        {
                            connectForm.StartGame = true;
                            break;
                        }
                    }
                }
            }
        }
        public void terminateConnection()
        {
            terminated = true;
            tcpClient.Close();
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
        }
    }
}
