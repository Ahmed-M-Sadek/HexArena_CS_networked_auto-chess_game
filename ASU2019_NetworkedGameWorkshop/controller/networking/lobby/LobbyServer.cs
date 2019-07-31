using ASU2019_NetworkedGameWorkshop.view;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ASU2019_NetworkedGameWorkshop.controller.networking.lobby
{
    class LobbyServer
    {
        private readonly string gameName;
        private readonly int port;
        private readonly ConnectForm connectForm;
        private readonly string playerName;
        private readonly List<Thread> threads;

        private bool terminated;
        private TcpListener tcpListener;
        private bool gameStartSent;

        public List<string> ConnectedIPs { get; }

        public LobbyServer(string gameName, int port, ConnectForm connectForm, string playerName)
        {
            this.gameName = gameName;
            this.port = port;
            this.connectForm = connectForm;
            this.playerName = playerName;
            ConnectedIPs = new List<string>();
            threads = new List<Thread>();
            terminated = false;
        }

        public void startServer()
        {
            Thread thread = new Thread(startNetworkListener)
            {
                IsBackground = true
            };
            threads.Add(thread);
            thread.Start();
        }

        private void startNetworkListener()
        {
            tcpListener = TcpListener.Create(port);
            tcpListener.Start();
            try
            {
                while (!terminated)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    Thread thread = new Thread(() => handleClient(tcpClient))
                    {
                        IsBackground = true
                    };
                    threads.Add(thread);
                    thread.Start();
                }
            }
            catch (SocketException)
            {
            }
        }

        private void handleClient(TcpClient tcpClient)
        {
            try
            {
                object lockObject = new object();

                using (StreamWriter streamWriter = new StreamWriter(tcpClient.GetStream()))
                using (StreamReader streamReader = new StreamReader(tcpClient.GetStream()))
                {
                    string connectionType = streamReader.ReadLine();
                    if (connectionType.Equals("PING"))
                    {
                        streamWriter.WriteLine(gameName + game.GameNetworkManager.NETWORK_MSG_SEPARATOR + playerName);
                        streamWriter.Flush();
                    }
                    else if (connectionType.Equals("LOBBY"))
                    {
                        streamWriter.WriteLine(playerName);
                        string clientIP = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString();
                        clientIP = clientIP.Substring(clientIP.LastIndexOf(':') + 1);
                        string remotePlayerName = streamReader.ReadLine();
                        connectForm.LobbyMembers.Enqueue($"{remotePlayerName}\t({clientIP})");

                        foreach (string ip in ConnectedIPs)
                        {
                            streamWriter.WriteLine("MEMBER#" + ip);
                        }
                        streamWriter.Flush();

                        lock (lockObject)
                        {
                            ConnectedIPs.Add(clientIP + game.GameNetworkManager.NETWORK_MSG_SEPARATOR + remotePlayerName);
                        }

                        int currentDevicesCount = ConnectedIPs.Count;
                        while (!terminated)
                        {
                            if (currentDevicesCount < ConnectedIPs.Count)
                            {
                                lock (lockObject)
                                {
                                    for (int i = currentDevicesCount; i < ConnectedIPs.Count; i++)
                                    {
                                        streamWriter.WriteLine("MEMBER#" + ConnectedIPs[i]);
                                    }
                                }
                                streamWriter.Flush();
                                currentDevicesCount = ConnectedIPs.Count;
                            }

                            if (connectForm.StartGame)
                            {
                                streamWriter.WriteLine("STARTGAME");
                                streamWriter.Flush();
                                gameStartSent = true;
                            }

                            Thread.Sleep(100);
                        }
                    }
                }
            }
            finally
            {
                tcpClient.Close();
            }
        }

        public void terminateConnection()
        {
            while (connectForm.StartGame && !gameStartSent)
            {
                Thread.Sleep(10);
            }
            terminated = true;
            tcpListener.Stop();
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
        }
    }
}
