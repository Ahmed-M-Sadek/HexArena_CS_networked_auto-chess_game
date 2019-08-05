using ASU2019_NetworkedGameWorkshop.controller.networking.game;
using ASU2019_NetworkedGameWorkshop.model;
using ASU2019_NetworkedGameWorkshop.model.character;
using ASU2019_NetworkedGameWorkshop.model.grid;
using ASU2019_NetworkedGameWorkshop.model.ui;
using ASU2019_NetworkedGameWorkshop.model.ui.shop.charactershop;
using System.Collections.Generic;
using System.Linq;
using static ASU2019_NetworkedGameWorkshop.model.ui.StageTimer;

namespace ASU2019_NetworkedGameWorkshop.controller
{
    public class StageManager
    {
        public enum GameStage { Buy, Fight, FightToBuy, BuyToFight }

        private readonly static int[] DMG_PER_CHARACTER_LEVEL = new int[] { 3, 5, 7 };

        private readonly StageTimer stageTimer;
        private readonly List<Character> teamBlue, teamRed;
        private readonly Grid grid;
        private readonly Dictionary<Character, Tile> charactersPrevPos;
        private readonly GameManager gameManager;
        private readonly GameNetworkManager gameNetworkManager;
        private readonly PlayersLeaderBoard playersLeaderBoard;
        private readonly CharShop charShop;
        private readonly Player player;

        public GameStage CurrentGameStage { get; set; }
        public int CurrentRound { get; set; }

        public StageManager(StageTimer stageTimer,
                            List<Character> teamBlue,
                            List<Character> teamRed,
                            Grid grid,
                            Player player,
                            PlayersLeaderBoard playersLeaderBoard,
                            CharShop charShop,
                            GameManager gameManager,
                            GameNetworkManager gameNetworkManager)
        {
            this.stageTimer = stageTimer;
            this.teamBlue = teamBlue;
            this.teamRed = teamRed;
            this.grid = grid;
            this.player = player;
            this.playersLeaderBoard = playersLeaderBoard;
            this.charShop = charShop;
            this.gameManager = gameManager;
            this.gameNetworkManager = gameNetworkManager;
            charactersPrevPos = new Dictionary<Character, Tile>();
            CurrentGameStage = GameStage.Buy;
            CurrentRound = 1;
        }


        public void switchStage()
        {
            if (CurrentGameStage == GameStage.Buy)
            {
                CurrentGameStage = GameStage.BuyToFight;
                stageTimer.resetTimer(StageTime.BUY_TO_FIGHT);
                System.Console.WriteLine("switched to " + StageTime.BUY_TO_FIGHT);
                gameManager.deselectSelectedTile();
                enqueueStageChangeMsg();
            }
            else if (CurrentGameStage == GameStage.Fight)
            {
                CurrentGameStage = GameStage.FightToBuy;
                stageTimer.resetTimer(StageTime.FIGHT_TO_BUY);
                enqueueStageChangeMsg();
            }
            else if (CurrentGameStage == GameStage.BuyToFight)
            {
                CurrentGameStage = GameStage.Fight;
                switchStageFight();
                enqueueStageChangeMsg();
            }
            else if (CurrentGameStage == GameStage.FightToBuy)
            {
                endRound();
                CurrentGameStage = GameStage.Buy;
                switchStageBuy();
                enqueueStageChangeMsg();
            }
        }

        private void enqueueStageChangeMsg ()
        {
            if(gameManager.IsHost)
                if(CurrentGameStage == GameStage.FightToBuy)
                {
                    if (gameManager.TeamBlue.Count(e => !e.IsDead) > 0)
                    {
                       gameNetworkManager.enqueueMsg(NetworkMsgPrefix.StageChange, networking.GameNetworkUtilities.serializeStage(CurrentGameStage, true));
                    }
                    else
                    {
                        gameNetworkManager.enqueueMsg(NetworkMsgPrefix.StageChange, networking.GameNetworkUtilities.serializeStage(CurrentGameStage, false));
                    }
                }
                else
                    gameNetworkManager.enqueueMsg(NetworkMsgPrefix.StageChange, networking.GameNetworkUtilities.serializeStage(CurrentGameStage, false));
        }

        private void switchStageBuy()
        {
            foreach (Character character in teamRed.Where(e => !e.IsDead))
            {
                character.CurrentTile.CurrentCharacter = null;
                if (character.ToMoveTo != null)
                {
                    character.ToMoveTo.Walkable = true;
                }
            }
            teamRed.Clear();
            foreach (Character character in teamBlue.Where(e => !e.IsDead))
            {
                character.CurrentTile.CurrentCharacter = null;
                if (character.ToMoveTo != null)
                {
                    character.ToMoveTo.Walkable = true;
                }
            }
            teamBlue.Clear();

            foreach (KeyValuePair<Character, Tile> characterPrevPos in charactersPrevPos)
            {
                if (characterPrevPos.Key.team == Character.Teams.Blue)
                {
                    teamBlue.Add(characterPrevPos.Key);
                }
                else
                {
                    teamRed.Add(characterPrevPos.Key);
                }
                characterPrevPos.Value.CurrentCharacter = characterPrevPos.Key;
                characterPrevPos.Key.reset();
            }
            grid.Transparent = false;

            charShop.refreshShop();

            stageTimer.resetTimer(StageTime.BUY);
        }

        private void switchStageFight()
        {
            charactersPrevPos.Clear();
            teamRed.ForEach(character => charactersPrevPos.Add(character, character.CurrentTile));
            teamBlue.ForEach(character => charactersPrevPos.Add(character, character.CurrentTile));

            grid.Transparent = true;

            stageTimer.resetTimer(StageTime.FIGHT);
        }

        private void endRound()
        {
            CurrentRound++;
            if (teamRed.Count(character => !character.IsDead) == 0)
            {
                player.incrementGold(Player.RoundEndStatus.WIN);
            }
            else
            {
                int dmg = 0;
                foreach (Character character in teamRed)
                {
                    if (character.IsDead)
                    {
                        continue;
                    }
                    dmg += DMG_PER_CHARACTER_LEVEL[character.CurrentLevel];
                }
                player.Health -= 2 + dmg;
                player.incrementGold(Player.RoundEndStatus.LOSS);
            }

            playersLeaderBoard.update();
            gameNetworkManager.enqueueMsg(networking.game.NetworkMsgPrefix.PlayerHealthUpdate,
                                          networking.GameNetworkUtilities.serializePlayerHP(player));
        }
    }
}
