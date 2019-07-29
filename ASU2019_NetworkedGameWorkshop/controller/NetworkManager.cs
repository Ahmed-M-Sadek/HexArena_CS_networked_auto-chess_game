using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using ASU2019_NetworkedGameWorkshop.model.character.types;
using ASU2019_NetworkedGameWorkshop.model.spell;

namespace ASU2019_NetworkedGameWorkshop.controller {
    public class NetworkManager {
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
            for(int i = 0; i < host.AddressList.Length; i++)
            {
                if(host.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    string ip = host.AddressList[i].ToString();
                    LocalIP.Add(ip);
                    LocalIPBase.Add(ip.Substring(0, ip.LastIndexOf('.') + 1));
                }
            }
        }

        public static Tuple<string, long>[] getServersInNetwork(int port)
        {
            ConcurrentBag<Tuple<string, long>> activeIPs = new ConcurrentBag<Tuple<string, long>>();
            CountdownEvent countdownEvent = new CountdownEvent((254 * LocalIPBase.Count) + 1);
            new Thread(new ThreadStart(() => {
                foreach(string ipBase in LocalIPBase)
                {
                    for(int i = 1; i < 255; i++)
                    {
                        Ping p = new Ping();
                        p.PingCompleted += new PingCompletedEventHandler((object sender, PingCompletedEventArgs e) => {
                            if(e.Reply != null && e.Reply.Status == IPStatus.Success)
                            {
                                try
                                {
                                    using(var client = new TcpClient())
                                    {
                                        IAsyncResult asyncResult = client.BeginConnect((string)e.UserState, port, null, null);
                                        if(asyncResult.AsyncWaitHandle.WaitOne(PORT_CHECK_TIMEOUT))
                                        {
                                            activeIPs.Add(Tuple.Create((string)e.UserState, e.Reply.RoundtripTime));
                                            client.EndConnect(asyncResult);
                                        }
                                    }
                                } catch(SocketException) { }
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

        public CharStat parseCharacter(string character)
        {
            String[] tokens = character.Split('#');

            List<(Spells[], int)> spellList = new List<(Spells[], int)>();
            int spellNum = Convert.ToInt32(tokens[4]);
            for(int i = 0; i < spellNum; i++)
            {
                spellList.Add((Spells.getSpell(Convert.ToInt32(tokens[5 + 2 * i])), Convert.ToInt32(tokens[6 + 2 * i])));
            }

            return new CharStat(
                CharacterType.getCharacterType(Convert.ToInt32(tokens[0])), Convert.ToInt32(tokens[1]),
                Convert.ToInt32(tokens[2]), Convert.ToInt32(tokens[3]), spellList);
        }

        public string serializeCharacter(model.character.Character character)
        {
            return "";
        }
        public void parseSkill(string character)
        {

        }

        public struct CharStat {
            public CharacterType[] charType;
            public int Level;
            public int X;
            public int Y;
            public List<(Spells[], int)> SpellList;

            public CharStat(CharacterType[] charType, int level, int x, int y, List<(Spells[], int)> spellList)
            {
                this.charType = charType;
                Level = level;
                X = x;
                Y = y;
                SpellList = spellList;
            }
        }

    }

}
