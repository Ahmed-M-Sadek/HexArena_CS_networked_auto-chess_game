﻿using ASU2019_NetworkedGameWorkshop.model.character.types;
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

        private const int PING_TIMEOUT = 999;

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

        public static Tuple<string, long, string>[] getServersInNetwork(int port)
        {
            ConcurrentBag<Tuple<string, long, string>> activeIPs = new ConcurrentBag<Tuple<string, long, string>>();
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
        public static string serializeCharacter(model.character.Character character)
        {
            string spells = "";
            foreach (var spell in character.LearnedSpells)
            {
                spells += $"#{spell.Id}#{spell.Level}";
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

        public static string serializeCharacterSwap(model.grid.Tile tile, model.grid.Tile selectedTile)
        {
            return $"{tile.X}#{tile.Y}#{selectedTile.X}#{selectedTile.Y}";
        }

        public static Tuple<model.grid.Tile, model.grid.Tile> parseCharacterSwap(string[] msg, model.grid.Grid grid)
        {
            return Tuple.Create(grid.Tiles[int.Parse(msg[1]), int.Parse(msg[2])],
                grid.Tiles[int.Parse(msg[3]), int.Parse(msg[4])]);
        }
    }
}