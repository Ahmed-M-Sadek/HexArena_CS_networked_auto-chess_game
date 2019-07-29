using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;

namespace ASU2019_NetworkedGameWorkshop.controller
{
    public class NetworkManager
    {
        public const int DEFAULT_PORT = 47999;

        private const int PING_TIMEOUT = 999;

        private static readonly TimeSpan PORT_CHECK_TIMEOUT = TimeSpan.FromSeconds(1);

        public static List<string> LocalIP { get; private set; }
        public static List<string> LocalIPBase { get; private set; }

        static NetworkManager()
        {
            LocalIP = new List<string>();
            LocalIPBase = new List<string>();
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            for (int i = 0; i < host.AddressList.Length; i++)
            {
                if (host.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    string ip = host.AddressList[i].ToString();
                    LocalIP.Add(ip);
                    LocalIPBase.Add(ip.Substring(0, ip.LastIndexOf('.') + 1));
                }
            }
        }

        public static void startServer(string gameName, int port, view.ConnectForm connectForm)
        {
            Thread thread = new Thread(() => startNetworkListener(gameName, port, connectForm))
            {
                IsBackground = true
            };
            thread.Start();
        }

        private static void startNetworkClient(string gameName)
        {
            throw new NotImplementedException();
        }

        private static void startNetworkListener(string gameName, int port, view.ConnectForm connectForm)
        {
            TcpListener tcpListener = TcpListener.Create(port);
            tcpListener.Start();

            try
            {
                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    Thread thread = new Thread(() => handleClient(gameName, tcpClient, connectForm))
                    {
                        IsBackground = true
                    };
                    thread.Start();
                }
            }
            catch (SocketException)
            {
                return;
            }
        }

        private static void handleClient(string gameName, TcpClient tcpClient, view.ConnectForm connectForm)
        {
            using (StreamWriter streamWriter = new StreamWriter(tcpClient.GetStream()))
            using (StreamReader streamReader = new StreamReader(tcpClient.GetStream()))
            {
                while (true)
                {
                    string msg = streamReader.ReadLine();
                    if (msg.Equals("PING"))
                    {
                        streamWriter.WriteLine(gameName);
                        streamWriter.Flush();//needed ?
                        break;
                    }

                    string ip = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString();
                    connectForm.LobbyMembers.Enqueue(ip.Substring(ip.LastIndexOf(':')+1));
                    streamWriter.WriteLine("LOBBY");
                    streamWriter.WriteLine(gameName);
                    streamWriter.Flush();//needed ?

                    break;
                }
            }
            tcpClient.Close();
        }


        public static void connectToServer(string ip, int port)
        {
            Thread thread = new Thread(() => handleServer(ip, port))
            {
                IsBackground = true
            };
            thread.Start();
        }

        private static void handleServer(string ip, int port)
        {
            using (TcpClient tcpClient = new TcpClient(ip, port))
            using (StreamReader streamReader = new StreamReader(tcpClient.GetStream()))
            using (StreamWriter streamWriter = new StreamWriter(tcpClient.GetStream()))
            {
                streamWriter.WriteLine("LOBBY");
                streamWriter.Flush();
                string msg = streamReader.ReadLine();
                if (msg.Equals("LOBBY"))
                {
                    Console.WriteLine(streamReader.ReadLine());
                }
            }
        }

        public static Tuple<string, long, string>[] getServersInNetwork(int port)
        {
            ConcurrentBag<Tuple<string, long, string>> activeIPs = new ConcurrentBag<Tuple<string, long, string>>();
            CountdownEvent countdownEvent = new CountdownEvent((254 * LocalIPBase.Count) + 1);
            new Thread(new ThreadStart(() =>
            {
                foreach (string ipBase in LocalIPBase)
                {
                    for (int i = 1; i < 255; i++)
                    {
                        Ping p = new Ping();
                        p.PingCompleted += new PingCompletedEventHandler((object sender, PingCompletedEventArgs e) =>
                        {
                            if (e.Reply != null && e.Reply.Status == IPStatus.Success)
                            {
                                try
                                {
                                    using (var client = new TcpClient())
                                    {
                                        IAsyncResult asyncResult = client.BeginConnect((string)e.UserState, port, null, null);
                                        if (asyncResult.AsyncWaitHandle.WaitOne(PORT_CHECK_TIMEOUT))
                                        {
                                            using (StreamWriter streamWriter = new StreamWriter(client.GetStream()))
                                            using (StreamReader streamReader = new StreamReader(client.GetStream()))
                                            {
                                                streamWriter.WriteLine("PING");
                                                streamWriter.Flush();
                                                activeIPs.Add(Tuple.Create((string)e.UserState, e.Reply.RoundtripTime, streamReader.ReadLine()));
                                                client.EndConnect(asyncResult);
                                            }
                                        }
                                    }
                                }
                                catch (SocketException) { }
                            }
                            countdownEvent.Signal();
                        });
                        string ip = ipBase + i.ToString();
                        p.SendAsync(ip, PING_TIMEOUT, ip);
                    }
                }
            })).Start();
            countdownEvent.Signal();
            countdownEvent.Wait();
            return activeIPs.ToArray();
        }
    }
}
