using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace ASU2019_NetworkedGameWorkshop.controller.networking.game
{
    internal abstract class GameNetworkManager
    {
        public ConcurrentQueue<string> DataToSend { get; }
        public ConcurrentQueue<string> DataReceived { get; }

        public GameNetworkManager()
        {
            DataToSend = new ConcurrentQueue<string>();
            DataReceived = new ConcurrentQueue<string>();
        }

        public abstract void start();

        protected void handleClientRead(TcpClient tcpClient)
        {
            try
            {
                using (StreamReader streamReader = new StreamReader(tcpClient.GetStream()))
                {
                    while (true)
                    {
                        DataReceived.Enqueue(streamReader.ReadLine());
                        Thread.Sleep(50);//??
                    }
                }
            }
            finally
            {
                tcpClient.Close();
            }
        }

        protected void handleClientWrite(TcpClient tcpClient)
        {

            try
            {
                using (StreamWriter streamWriter = new StreamWriter(tcpClient.GetStream()))
                {
                    while (true)
                    {
                        if (DataToSend.Count > 0)
                        {
                            while (DataToSend.Count > 0)
                            {
                                DataToSend.TryDequeue(out string result);
                                streamWriter.WriteLine(result);
                            }
                            streamWriter.Flush();
                        }

                        Thread.Sleep(50);//??
                    }
                }
            }
            finally
            {
                tcpClient.Close();
            }
        }

        protected void startThreads(TcpClient tcpClient)
        {
            Thread readThread = new Thread(() => handleClientRead(tcpClient))
            {
                IsBackground = true
            };
            readThread.Start();
            Thread writeThread = new Thread(() => handleClientWrite(tcpClient))
            {
                IsBackground = true
            };
            writeThread.Start();
        }
    }
}