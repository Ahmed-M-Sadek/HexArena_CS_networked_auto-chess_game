using System;
using System.Drawing;

namespace ASU2019_NetworkedGameWorkshop.model
{
    public class Player : GraphicsObject
    {
        public enum RoundEndStatus { WIN, LOSS }

        private const int BASE_GOLD_INCOME = 5;

        private static readonly Pen darkRedPen;

        private int health = 100;
        private int gold;
        private int streak;
        private RoundEndStatus streakStatus;

        public int Health
        {
            get { return health; }
            set
            {
                if (value < 0)
                {
                    health = 0;
                }
                else
                {
                    health = value;
                }
            }
        }

        static Player()
        {
            darkRedPen = Pens.DarkRed.Clone() as Pen;
            darkRedPen.Width = 4;
        }

        public void incrementGold(RoundEndStatus roundEndStatus)
        {
            gold += BASE_GOLD_INCOME 
                + getstreakIncome() 
                + Math.Min(5, gold/10)
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

        public override void draw(Graphics graphics)
        {
            graphics.DrawString(health.ToString(), new Font("Roboto", 12, FontStyle.Bold), Brushes.DarkRed, 200, 15);//temp pos
            graphics.DrawArc(darkRedPen, 200-7, 15-12, 50, 50, 180, -360 * health / 100);//temp pos
            graphics.DrawString("Gold: "+gold, new Font("Roboto", 12, FontStyle.Bold), Brushes.Yellow, 260, 15);//temp pos
            graphics.DrawString("Streak: " + streak, new Font("Roboto", 12, FontStyle.Bold),
                streakStatus == RoundEndStatus.WIN ? Brushes.OrangeRed : Brushes.SteelBlue, 350, 15);//temp pos
        }
    }
}
