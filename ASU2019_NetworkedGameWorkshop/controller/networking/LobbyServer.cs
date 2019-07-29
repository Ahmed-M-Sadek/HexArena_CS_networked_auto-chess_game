using ASU2019_NetworkedGameWorkshop.view;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ASU2019_NetworkedGameWorkshop.controller.networking
{
    class LobbyServer
    {
        private readonly string gameName;
        private readonly int port;
        private readonly ConnectForm connectForm;

        public List<string> ConnectedIPs { get; }

        public LobbyServer(string gameName, int port, ConnectForm connectForm)
        {
            this.gameName = gameName;
            this.port = port;
            this.connectForm = connectForm;

            ConnectedIPs = new List<string>();
        }

        public void startServer()
        {
            new Thread(startNetworkListener)
            {
                IsBackground = true
            }.Start();
        }

        private void startNetworkListener()
        {
            TcpListener tcpListener = TcpListener.Create(port);
            tcpListener.Start();

            while (true)
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();

                Thread thread = new Thread(() => handleClient(tcpClient))
                {
                    IsBackground = true
                };
                thread.Start();
            }
        }

        private void handleClient(TcpClient tcpClient)
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
                    while (true)
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

                        Thread.Sleep(100);
                    }
                }
            }
            tcpClient.Close();
        }
    }
}
