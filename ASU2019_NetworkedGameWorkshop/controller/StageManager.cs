using ASU2019_NetworkedGameWorkshop.model.character;
using ASU2019_NetworkedGameWorkshop.model.grid;
using ASU2019_NetworkedGameWorkshop.model.ui;
using System.Collections.Generic;
using System.Linq;
using static ASU2019_NetworkedGameWorkshop.model.ui.StageTimer;

namespace ASU2019_NetworkedGameWorkshop.controller
{
    public class StageManager
    {
        public enum GameStage { Buy, Fight, FightToBuy, BuyToFight }

        private readonly StageTimer stageTimer;
        private readonly List<Character> teamBlue, teamRed;
        private readonly Grid grid;
        private readonly Dictionary<Character, Tile> charactersPrevPos;
        private readonly GameManager gameManager;

        public GameStage CurrentGameStage { get; private set; }

        public StageManager(StageTimer stageTimer,
                            List<Character> teamBlue,
                            List<Character> teamRed,
                            Grid grid,
                            GameManager gameManager)
        {
            this.stageTimer = stageTimer;
            this.teamBlue = teamBlue;
            this.teamRed = teamRed;
            this.grid = grid;
            this.gameManager = gameManager;

            charactersPrevPos = new Dictionary<Character, Tile>();
            CurrentGameStage = GameStage.Buy;
        }

        public void switchStage()
        {
            if (CurrentGameStage == GameStage.Buy)
            {
                CurrentGameStage = GameStage.BuyToFight;
                stageTimer.resetTimer(StageTime.BUY_TO_FIGHT);
                gameManager.deselectSelectedTile();
            }
            else if (CurrentGameStage == GameStage.Fight)
            {
                CurrentGameStage = GameStage.FightToBuy;
                stageTimer.resetTimer(StageTime.FIGHT_TO_BUY);
            }
            else if (CurrentGameStage == GameStage.BuyToFight)
            {
                CurrentGameStage = GameStage.Fight;
                switchStageFight();
            }
            else if (CurrentGameStage == GameStage.FightToBuy)
            {
                CurrentGameStage = GameStage.Buy;
                switchStageBuy();
            }
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
    }
}
