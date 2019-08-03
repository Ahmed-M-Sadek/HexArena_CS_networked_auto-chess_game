using ASU2019_NetworkedGameWorkshop.controller;
using System.Drawing;

namespace ASU2019_NetworkedGameWorkshop.model.ui
{
    public class StageTimer : GraphicsObject
    {
        public enum StageTime
        {
            FIGHT = 41 * 1000,
            BUY = 31 * 1000,
            DEBUGGING = 6 * 1000,
            FIGHT_TO_BUY = 6 * 1000,
            BUY_TO_FIGHT = 6 * 1000,
        }

        public delegate void SwitchStage();

        private static readonly Font FONT = new Font("Roboto", 14f);

        private readonly GameManager gameManager;

        private StageTime currentStageTime;
        private long timerEnd;
        private long currentTime;
        private bool isFirstTimeSound = true;

        /// <summary>
        /// Method called if the timer reaches zero or ends.
        /// </summary>
        public SwitchStage switchStageEvent { get; set; }

        public StageTimer(GameManager gameManager) : this(gameManager, null) { }

        public StageTimer(GameManager gameManager, SwitchStage switchStage)
        {
            this.gameManager = gameManager;
            this.switchStageEvent = switchStage;
        }

        public void resetTimer(StageTime stageTime)
        {
            currentStageTime = stageTime;
            timerEnd = (int)(stageTime) + gameManager.ElapsedTime;
        }

        public bool update()
        {
            if (timerEnd < gameManager.ElapsedTime)
            {
                switchStageEvent();
                isFirstTimeSound = true;
                return true;
            }
            long newTime = (timerEnd - gameManager.ElapsedTime) / 1000;
            if (newTime == 9 && currentStageTime == StageTime.BUY && isFirstTimeSound) {
                SoundManager.PlaySound("10Seconds.wav");
                isFirstTimeSound = false;
            }
            if (currentTime == newTime)
            {
                return false;
            }
            else
            {
                currentTime = newTime;
                return true;
            }
        }

        public void endTimer()
        {
            switchStageEvent();
        }

        public override void draw(Graphics graphics)
        {
            graphics.DrawString(currentStageTime + " - Time Left: " + currentTime, FONT,
                Brushes.Black, 500, 15);//temp
        }

        public override void drawDebug(Graphics graphics)
        {
        }
    }
}
