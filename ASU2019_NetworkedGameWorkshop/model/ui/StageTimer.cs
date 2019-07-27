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

        private readonly GameManager gameManager;
        private readonly SwitchStage switchStage;

        private StageTime currentStageTime;
        private long timerEnd;
        private long currentTime;

        public StageTimer(GameManager gameManager, SwitchStage switchStage)
        {
            this.gameManager = gameManager;
            this.switchStage = switchStage;
        }

        public void resetTimer(StageTime stageTime)
        {
            currentStageTime = stageTime;
            timerEnd = (int)(stageTime) + gameManager.ElapsedTime;
        }

        public bool update()
        {
            long newTime = (timerEnd - gameManager.ElapsedTime) / 1000;
            if (currentTime == newTime)
            {
                return false;
            }
            else
            {
                if (newTime == 0)
                {
                    switchStage();
                }
                currentTime = newTime;
                return true;
            }
        }

        public override void draw(Graphics graphics)
        {
            graphics.DrawString(currentStageTime + " - Time Left: " + currentTime, new Font("Roboto", 14f),
                Brushes.Black, 500, 15);//temp
        }

        public void endTimer()
        {
            switchStage();
        }
    }
}
