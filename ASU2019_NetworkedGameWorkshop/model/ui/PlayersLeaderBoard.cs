using System.Collections.Generic;
using System.Drawing;

namespace ASU2019_NetworkedGameWorkshop.model.ui
{
    /// <summary>
    /// <para>A class that displays an ordered list of health bars from a given list of players to the right of the screen.
    /// incase of 2 or more players having the same healthpoints then the local player will be the first on the leaderboard.</para>
    /// 
    /// <para>This class only uses the health property of the Player so an incomplete player can be passed as long as the health is correct.</para>
    /// </summary>
    class PlayersLeaderBoard : GraphicsObject
    {
        private const int X = 30,
            Y_START = 100, CIRCLE_DIM = 50, ENTRY_PADDING = 10;

        private static readonly Font FONT;
        private static readonly Pen PEN_DARK_RED;

        private readonly List<Player> players;

        static PlayersLeaderBoard()
        {
            FONT = new Font("Roboto", 12, FontStyle.Bold);
            PEN_DARK_RED = Pens.DarkRed.Clone() as Pen;
            PEN_DARK_RED.Width = 4;
        }

        public PlayersLeaderBoard(List<Player> players)
        {
            this.players = players;

            update();
        }

        public PlayersLeaderBoard(params Player[] players)
        {
            this.players = new List<Player>(players);

            update();
        }

        /// <summary>
        /// Add Players to the leaderboard
        /// </summary>
        /// <param name="players">players to add</param>
        public void addPlayers(params Player[] players)
        {
            this.players.AddRange(players);
        }

        /// <summary>
        /// remove Players to the leaderboard
        /// </summary>
        /// <param name="players">players to remove</param>
        public void removePlayers(params Player[] players)
        {
            foreach (Player player in players)
            {
                this.players.Remove(player);
            }
        }

        /// <summary>
        /// <para>Resort the list of players based on health</para>
        /// <para>should be called after a change to players' health was made </para>
        /// </summary>
        public void update()
        {
            players.Sort((p1, p2) =>
            {
                int diff = p2.Health - p1.Health;
                return diff == 0 ? 1 : diff;
            });
        }

        /// <summary>
        /// Draws the player name, Health points, and a circle indicating the health points around the HP label
        /// </summary>
        /// <param name="graphics">graphics object to draw on</param>
        public override void draw(Graphics graphics)
        {
            for (int i = 0; i < players.Count; i++)
            {
                int y = Y_START + (CIRCLE_DIM + ENTRY_PADDING) * i;
                Brush fontBrush = players[i].Local ? Brushes.Gold : Brushes.DarkRed;
                graphics.DrawString(players[i].Health.ToString(), FONT, fontBrush, X, y);
                graphics.DrawArc(PEN_DARK_RED, X - 11, y - 15, CIRCLE_DIM, CIRCLE_DIM, 180, -360 * players[i].Health / Player.MAX_HP);//temp padding
                graphics.DrawString(players[i].Name, new Font("Roboto", 8, FontStyle.Bold), fontBrush, X + CIRCLE_DIM + 2, y + 8 / 2);//temp padding
            }
        }
    }
}
