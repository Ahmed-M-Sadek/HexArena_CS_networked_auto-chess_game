using ASU2019_NetworkedGameWorkshop.controller;
using ASU2019_NetworkedGameWorkshop.controller.networking;
using ASU2019_NetworkedGameWorkshop.controller.networking.game;
using System;
using System.Drawing;

namespace ASU2019_NetworkedGameWorkshop.model.ui
{
    public class StageTimer : GraphicsObject
    {
        public enum StageTime
        {
            FIGHT = 31 * 1000,
            BUY = 41 * 1000,
            DEBUGGING = 6 * 1000,
            FIGHT_TO_BUY = 6 * 1000,
            BUY_TO_FIGHT = 6 * 1000,
        }

        public delegate void SwitchStage();

        private static readonly Font FONT = new Font("Roboto", 14f);

        private readonly GameManager gameManager;
        private readonly GameNetworkManager gameNetworkManager;
        private StageTime currentStageTime;
        private long timerEnd;
        private long currentTime;

        /// <summary>
        /// Method called if the timer reaches zero or ends.
        /// </summary>
        public SwitchStage switchStageEvent { get; set; }

        public bool CanSwitch { get; set; }
        public long NextTimerEndSystemTime { get; set; }

        public StageTimer(GameManager gameManager, GameNetworkManager gameNetworkManager) : this(gameManager, null, gameNetworkManager) { }

        public StageTimer(GameManager gameManager, SwitchStage switchStage, GameNetworkManager gameNetworkManager)
        {
            this.gameManager = gameManager;
            this.switchStageEvent = switchStage;
            this.gameNetworkManager = gameNetworkManager;
        }

        public void startTimer(StageTime stageTime)
        {
            currentStageTime = stageTime;
            timerEnd = (int)stageTime + gameManager.ElapsedTime;
        }

        public void resetTimer(StageTime stageTime)
        {
            currentStageTime = stageTime;
            if (gameManager.IsHost)
            {
                timerEnd = (int)stageTime + gameManager.ElapsedTime;
                gameNetworkManager.enqueueMsg(NetworkMsgPrefix.RoundEndSync,
                                              GameNetworkUtilities.serializeRoundEndTime(
                                                  (int)stageTime + (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond)));
            }
            else
            {
                timerEnd = NextTimerEndSystemTime - (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) + gameManager.ElapsedTime;
                CanSwitch = false;
            }
        }

        public bool update()
        {
            if (timerEnd < gameManager.ElapsedTime)
            {
                if (gameManager.IsHost || CanSwitch)
                {
                    switchStageEvent();
                }
                return true;
            }
            long newTime = (timerEnd - gameManager.ElapsedTime) / 1000;
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
            while (true)
            {
                if (gameManager.IsHost || CanSwitch)
                {
                    switchStageEvent();
                    break;
                }
            }
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
