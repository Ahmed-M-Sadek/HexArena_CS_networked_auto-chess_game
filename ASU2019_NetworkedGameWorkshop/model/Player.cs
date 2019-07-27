using System;
using System.Drawing;

namespace ASU2019_NetworkedGameWorkshop.model
{
    /// <summary>
    /// A class representing the player which hold the current health points, gold, and streak
    /// </summary>
    public class Player : GraphicsObject
    {
        public enum RoundEndStatus { WIN, LOSS }

        public const int MAX_HP = 100;

        private const int BASE_GOLD_INCOME = 5;

        private static readonly Font FONT = new Font("Roboto", 12, FontStyle.Bold);

        private int health = MAX_HP;
        private int gold;
        private int streak;
        private RoundEndStatus streakStatus;

        /// <summary>
        /// <para>The Player's health points</para>
        /// 
        /// The health points will never go below zero.
        /// </summary>
        public int Health
        {
            get { return health; }
            set { health = (value < 0) ? 0 : value; }
        }

        public string Name { get; }
        /// <summary>
        /// Represents whether is player represents the local player of the machine or a player on another machine
        /// </summary>
        public bool Local { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="local">Represents whether is player represents the local player of the machine or a player on another machine</param>
        public Player(string name, bool local = false)
        {
            Name = name;
            Local = local;
        }

        /// <summary>
        /// Increases the player's gold based on players win streak, whether the player won or lost the last round and gold interest.
        /// 
        /// <para>The streak is incremented after increasing the player's gold</para>
        /// </summary>
        /// <param name="roundEndStatus">the status of the last round</param>
        public void incrementGold(RoundEndStatus roundEndStatus)
        {
            gold += BASE_GOLD_INCOME
                + getstreakIncome()
                + Math.Min(5, gold / 10)
                + ((roundEndStatus == RoundEndStatus.WIN) ? 1 : 0);
            if (streakStatus == roundEndStatus)
            {
                streak++;
            }
            else
            {
                streakStatus = roundEndStatus;
                streak = 1;
            }
        }

        private int getstreakIncome()
        {
            if (streak > 6)
            {
                return 3;
            }
            else if (streak > 3)
            {
                return 2;
            }
            else if (streak > 1)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Draws labels for the Player's Gold and Win/Lose Streak (colored accordingly)
        /// </summary>
        /// <param name="graphics">graphics object to draw on</param>
        public override void draw(Graphics graphics)
        {
            graphics.DrawString("Gold: " + gold, FONT, Brushes.DarkGoldenrod, 260, 15);//temp pos
            graphics.DrawString("Streak: " + streak, FONT,
                streakStatus == RoundEndStatus.WIN ? Brushes.OrangeRed : Brushes.SteelBlue, 350, 15);//temp pos
        }

        public override void drawDebug(Graphics graphics)
        {
        }
    }
}
