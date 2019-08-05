using ASU2019_NetworkedGameWorkshop.model;
using ASU2019_NetworkedGameWorkshop.model.character;
using ASU2019_NetworkedGameWorkshop.model.character.types;
using ASU2019_NetworkedGameWorkshop.model.grid;
using ASU2019_NetworkedGameWorkshop.model.spell;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;

namespace ASU2019_NetworkedGameWorkshop.controller.networking
{
    public class GameNetworkUtilities
    {
        public const int DEFAULT_PORT = 47999;

        private const int PING_TIMEOUT = 9999;

        public static readonly ServerStats INVALID_SERVER = new ServerStats("INVALID_SERVER", -1, "INVALID_SERVER", "INVALID_SERVER");

        private static readonly TimeSpan PORT_CHECK_TIMEOUT = TimeSpan.FromSeconds(1);

        public static List<string> LocalIP { get; private set; }
        public static List<string> LocalIPBase { get; private set; }

        static GameNetworkUtilities()
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

        public static ServerStats[] getServersInNetwork(int port)
        {
            ConcurrentBag<ServerStats> activeIPs = new ConcurrentBag<ServerStats>();
            CountdownEvent countdownEvent = new CountdownEvent(254 * LocalIPBase.Count + 1);
            new Thread(new ThreadStart(() =>
            {
                foreach (string ipBase in LocalIPBase)
                {
                    for (int i = 1; i < 255; i++)
                    {
                        Ping p = new Ping();
                        p.PingCompleted += new PingCompletedEventHandler((sender, e) =>
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
                                                string[] serverReply = streamReader.ReadLine().Split(game.GameNetworkManager.NETWORK_MSG_SEPARATOR);
                                                activeIPs.Add(new ServerStats((string)e.UserState, e.Reply.RoundtripTime, serverReply[0], serverReply[1]));
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
        /// <summary>
        /// Attempts to ping a given ip on a given port.
        /// <para>returns GameNetworkUtilities.INVALID_SERVER incase of failure.</para>
        /// </summary>
        /// <param name="ip">IP to ping.</param>
        /// <param name="port">the ip's open port to ping.</param>
        /// <returns>ServerStats of the server or GameNetworkUtilities.INVALID_SERVER incase of failure.</returns>
        public static ServerStats pingIP(string ip, int port)
        {
            ServerStats serverStats = INVALID_SERVER;

            using (Ping ping = new Ping())
            {
                PingReply pingReply = ping.Send(ip, PING_TIMEOUT);
                if (pingReply != null && pingReply.Status == IPStatus.Success)
                {
                    try
                    {
                        using (var client = new TcpClient())
                        {
                            IAsyncResult asyncResult = client.BeginConnect(ip, port, null, null);
                            if (asyncResult.AsyncWaitHandle.WaitOne(PORT_CHECK_TIMEOUT))
                            {
                                using (StreamWriter streamWriter = new StreamWriter(client.GetStream()))
                                using (StreamReader streamReader = new StreamReader(client.GetStream()))
                                {
                                    streamWriter.WriteLine("PING");
                                    streamWriter.Flush();
                                    string[] serverReply = streamReader.ReadLine().Split(game.GameNetworkManager.NETWORK_MSG_SEPARATOR);
                                    client.EndConnect(asyncResult);
                                    serverStats = new ServerStats(ip, pingReply.RoundtripTime, serverReply[0], serverReply[1]);
                                }
                            }
                        }
                    }
                    catch (SocketException) { }
                }
            }
            return serverStats;
        }

        /// <summary>
        /// takes a string and parses it returning CharStat
        /// token sequence: CharType, CharLvl, X, Y, number of skill, skill, lvl, skill, lvl...
        /// tokens seperated by a '#'
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public static CharStat parseCharacter(string[] character)
        {
            List<(Spells[], int)> spellList = new List<(Spells[], int)>();
            int spellNum = Convert.ToInt32(character[5]);
            for (int i = 0; i < spellNum; i++)
            {
                spellList.Add((Spells.getSpell(Convert.ToInt32(character[6 + 2 * i])), Convert.ToInt32(character[7 + 2 * i])));
            }

            return new CharStat(
                CharacterType.getCharacterType(Convert.ToInt32(character[1])), Convert.ToInt32(character[2]),
                Convert.ToInt32(character[3]), Convert.ToInt32(character[4]), spellList);
        }

        /// <summary>
        /// takes a Character and serializes it returning a string
        /// token sequence: CharType, CharLvl, X, Y, number of skill, skill, lvl, skill, lvl...
        /// tokens seperated by a '#'
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public static string serializeCharacter(Character character)
        {
            string spells = "";
            foreach (Spells[] spell in character.LearnedSpells)
            {
                spells += $"#{spell[character.SpellLevel[spell]].Id}#{character.SpellLevel[spell]}";
            }
            return $"{character.CharacterType.Id}#{character.CurrentLevel}#{character.CurrentTile.X}#{character.CurrentTile.Y}#{character.LearnedSpells.Count}{spells}";
        }
        public void parseSkill(string character)
        {

        }

        public struct CharStat
        {
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

        internal static string serializePlayerHP(Player player)
        {
            return $"{player.Name}#{player.Health}";
        }

        internal static string serializeTile(Tile tile)
        {
            return $"{tile.X}#{tile.Y}";
        }
        internal static string serializeSpellActionMoving(Spells[] spell, int index)
        {
            return $"{index}#{spell[0].Id}";
        }
        internal static string serializeSpellAction(Spells[] spell, Tile tile)
        {
            return $"{tile.X}#{tile.Y}#{spell[0].Id}";
        }
        internal static string serializeSpellSwap(int spellIndex, int charIndex)
        {
            return $"{charIndex}#{spellIndex}";
        }

        public static string serializeCharacterSwap(Tile tile, Tile selectedTile)
        {
            return $"{tile.X}#{tile.Y}#{selectedTile.X}#{selectedTile.Y}";
        }

        public static string serializeStage(StageManager.GameStage gameStage, bool BlueWins)
        {
            switch (gameStage)
            {
                case StageManager.GameStage.Buy:
                    return $"B#{BlueWins}";
                case StageManager.GameStage.BuyToFight:
                    return $"BF#{BlueWins}";
                case StageManager.GameStage.Fight:
                    return $"F#{BlueWins}";
                case StageManager.GameStage.FightToBuy:
                    return $"FB#{BlueWins}";
            }
            return $"B#{BlueWins}";
        }

        public static (StageManager.GameStage, bool) parseStage(string stage, string hostWins)
        {
            switch (stage)
            {
                case "B":
                    return (StageManager.GameStage.Buy, true);
                case "BF":
                    return (StageManager.GameStage.BuyToFight, true);
                case "F":
                    return (StageManager.GameStage.Fight, true);
                case "FB":
                    return (StageManager.GameStage.FightToBuy, Convert.ToBoolean(hostWins));
            }
            return (StageManager.GameStage.Buy, true);
        }

        public static Tuple<Tile, Tile> parseCharacterSwap(string[] msg, model.grid.Grid grid)
        {
            return Tuple.Create(grid.Tiles[int.Parse(msg[1]), int.Parse(msg[2])],
                grid.Tiles[int.Parse(msg[3]), int.Parse(msg[4])]);
        }

        public struct ServerStats
        {
            public ServerStats(string ip, long ping, string gameName, string hostPlayerName)
            {
                Ping = ping;
                GameName = gameName;
                Ip = ip;
                HostPlayerName = hostPlayerName;
            }

            public long Ping { get; set; }
            public string GameName { get; set; }
            public string Ip { get; set; }
            public string HostPlayerName { get; set; }
        }
    }
}
