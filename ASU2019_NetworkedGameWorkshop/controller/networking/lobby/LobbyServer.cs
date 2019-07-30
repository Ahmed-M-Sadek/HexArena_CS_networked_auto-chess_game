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
        private readonly List<Thread> threads;

        private bool terminated;
        private TcpListener tcpListener;
        private bool gameStartSent;

        public List<string> ConnectedIPs { get; }

        public LobbyServer(string gameName, int port, ConnectForm connectForm)
        {
            this.gameName = gameName;
            this.port = port;
            this.connectForm = connectForm;

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
                    if (streamReader.ReadLine().Equals("PING"))
                    {
                        streamWriter.WriteLine(gameName);
                        streamWriter.Flush();
                    }
                    else
                    {
                        string clientIP = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString();
                        clientIP = clientIP.Substring(clientIP.LastIndexOf(':') + 1);
                        connectForm.LobbyMembers.Enqueue(clientIP);

                        foreach (string ip in ConnectedIPs)
                        {
                            streamWriter.WriteLine("MEMBER#" + ip);
                        }
                        streamWriter.Flush();

                        lock (lockObject)
                        {
                            ConnectedIPs.Add(clientIP);
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
