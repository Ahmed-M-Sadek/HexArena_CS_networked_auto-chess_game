using ASU2019_NetworkedGameWorkshop.model.character;
using ASU2019_NetworkedGameWorkshop.model.character.types;
using ASU2019_NetworkedGameWorkshop.model.grid;
using ASU2019_NetworkedGameWorkshop.model.spell;
using ASU2019_NetworkedGameWorkshop.model.ui;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using static ASU2019_NetworkedGameWorkshop.model.ui.StageTimer;

namespace ASU2019_NetworkedGameWorkshop.controller
{
    public class GameManager
    {
        private enum GameStage { Buy, Fight, FightToBuy, BuyToFight }

        private const int GAMELOOP_INTERVAL = 50, TICK_INTERVAL = 1000;
        private const int GRID_HEIGHT = 6, GRID_WIDTH = 7;

        private readonly Grid grid;
        private readonly GameForm gameForm;
        private readonly Timer timer;
        private readonly StageTimer stageTimer;
        private readonly Stopwatch stopwatch;
        private readonly Dictionary<Character, Tile> charactersPrevPos;

        private Tile selectedTile;
        private long nextTickTime;
        private GameStage gameStage;

        public long ElapsedTime
        {
            get
            {
                return stopwatch.ElapsedMilliseconds;
            }
        }
        public List<Character> TeamBlue { get; private set; }
        public List<Character> TeamRed { get; private set; }

        public GameManager(GameForm gameForm)
        {
            this.gameForm = gameForm;
            grid = new Grid(GRID_WIDTH, GRID_HEIGHT,
                (int)((gameForm.Width - (Tile.WIDTH * GRID_WIDTH)) / 2),
                (int)((gameForm.Height - (Tile.HEIGHT * GRID_HEIGHT)) / 2) + 30);//temp values 

            TeamBlue = new List<Character>();
            TeamRed = new List<Character>();

            gameStage = GameStage.Buy;
            stageTimer = new StageTimer(this, switchStage);
            charactersPrevPos = new Dictionary<Character, Tile>();

            stopwatch = new Stopwatch();
            timer = new Timer
            {
                Interval = GAMELOOP_INTERVAL //Arbitrary: 20 ticks per sec
            };
            timer.Tick += new EventHandler(gameLoop);


            //Debugging 
            Character red = new Character(grid, grid.Tiles[6, 5], Character.Teams.Red, CharacterTypePhysical.Archer, this);
            red.learnSpell(Spells.AwesomeFireballAOE);
            red.learnSpell(Spells.Execute);
            red.learnSpell(Spells.Heal);
            Character blue1 = new Character(grid, grid.Tiles[2, 0], Character.Teams.Blue, CharacterTypePhysical.Archer, this);
            Character blue2 = new Character(grid, grid.Tiles[3, 0], Character.Teams.Blue, CharacterTypePhysical.Archer, this);
            Character blue3 = new Character(grid, grid.Tiles[4, 0], Character.Teams.Blue, CharacterTypePhysical.Archer, this);
            blue1.takeDamage(200,model.DamageType.MagicDamage);
            TeamRed.Add(red);
            TeamBlue.Add(blue1);
            TeamBlue.Add(blue2);
            TeamBlue.Add(blue3)
        }

        public void addRangeToForm(params Control[] control)
        {
            gameForm.Controls.AddRange(control);
        }
        public void removeRangeFromForm(params Control[] control)
        {
            gameForm.Controls.Remove(control);
        }

        private void switchStage()
        {
            if (gameStage == GameStage.Buy)
            {
                gameStage = GameStage.BuyToFight;
                stageTimer.resetTimer(StageTime.BUY_TO_FIGHT);
            }
            else if (gameStage == GameStage.Fight)
            {
                gameStage = GameStage.FightToBuy;
                stageTimer.resetTimer(StageTime.FIGHT_TO_BUY);
            }
            else if (gameStage == GameStage.BuyToFight)
            {
                gameStage = GameStage.Fight;
                switchStageFight();
            }
            else if (gameStage == GameStage.FightToBuy)
            {
                gameStage = GameStage.Buy;
                switchStageBuy();
            }
        }

        private void switchStageBuy()
        {
            foreach (Character character in TeamRed)
            {
                character.CurrentTile.CurrentCharacter = null;
            }
            TeamRed.Clear();
            foreach (Character character in TeamBlue)
            {
                character.CurrentTile.CurrentCharacter = null;
            }
            TeamBlue.Clear();

            foreach (KeyValuePair<Character, Tile> characterPrevPos in charactersPrevPos)
            {
                if (characterPrevPos.Key.team == Character.Teams.Blue)
                {
                    TeamBlue.Add(characterPrevPos.Key);
                }
                else
                {
                    TeamRed.Add(characterPrevPos.Key);
                }
                characterPrevPos.Value.CurrentCharacter = characterPrevPos.Key;
                characterPrevPos.Key.reset();
            }
            foreach (Tile tile in grid.Tiles)
            {
                tile.Transparent = false;
            }
            stageTimer.resetTimer(StageTime.BUY);
        }

        private void switchStageFight()
        {
            charactersPrevPos.Clear();
            foreach (Character character in TeamRed)
            {
                charactersPrevPos.Add(character, character.CurrentTile);
            }
            foreach (Character character in TeamBlue)
            {
                charactersPrevPos.Add(character, character.CurrentTile);
            }

            foreach (Tile tile in grid.Tiles)
            {
                tile.Transparent = true;
            }

            if (selectedTile != null)
            {
                selectedTile.Selected = false;
                selectedTile = null;
            }
            stageTimer.resetTimer(StageTime.FIGHT);
        }

        public void startTimer()
        {
            gameStart();
            timer.Start();
        }

        public void mouseClick(MouseEventArgs e)
        {
            if (gameStage == GameStage.Buy)
            {
                if (e.Button == MouseButtons.Right)
                {
                    selectedTile.Selected = false;
                    selectedTile = null;
                    gameForm.Invalidate();
                }
                else if (e.Button == MouseButtons.Left)
                {
                    Tile tile = grid.getSelectedHexagon(e.X, e.Y);
                    if (tile != null)
                    {
                        selectTile(tile);
                    }
                }
            }
        }

        private void selectTile(Tile tile)
        {
            if (selectedTile == tile)
            {
                selectedTile.Selected = false;
                selectedTile = null;
            }
            else if (selectedTile == null)
            {
                selectedTile = tile;
                selectedTile.Selected = true;
            }
            else
            {
                selectedTile.Selected = false;
                Character temp = selectedTile.CurrentCharacter;
                selectedTile.CurrentCharacter = tile.CurrentCharacter;
                tile.CurrentCharacter = temp;
                selectedTile = null;
            }

            gameForm.Invalidate();
        }

        public void updatePaint(PaintEventArgs e)
        {
            grid.draw(e.Graphics);

            TeamBlue.ForEach(character => character.draw(e.Graphics));
            TeamRed.ForEach(character => character.draw(e.Graphics));

            stageTimer.draw(e.Graphics);
        }

        private void gameStart()
        {
            stopwatch.Start();
            stageTimer.resetTimer(StageTime.DEBUGGING);//Debugging
            //stageTimer.resetTimer(StageTime.BUY_TIME);
        }

        private void gameLoop(object sender, EventArgs e)
        {
            bool updateCanvas = stageTimer.update();

            if (gameStage == GameStage.Buy)
            {
                updateCanvas = stageUpdateBuy() || updateCanvas;
            }
            else if (gameStage == GameStage.Fight)
            {
                updateCanvas = stageUpdateFight() || updateCanvas;
            }

            if (updateCanvas)
                gameForm.Invalidate();
        }

        private bool stageUpdateBuy()
        {
            bool updateCanvas = false;
            return updateCanvas;
        }

        private bool stageUpdateFight()
        {
            if (TeamBlue.Count == 0 || TeamRed.Count == 0)
            {
                stageTimer.endTimer();
                return true;
            }

            bool updateCanvas = false;
            bool predicate(Character character)
            {
                if (character.IsDead)
                {
                    updateCanvas = true;
                    return false;
                }
                return true;
            }
            foreach (Character character in TeamBlue)
            {
                updateCanvas = character.update() || updateCanvas;
            }
            TeamBlue = TeamBlue.Where(predicate).ToList();
            foreach (Character character in TeamRed)
            {
                updateCanvas = character.update() || updateCanvas;
            }
            TeamRed = TeamRed.Where(predicate).ToList();

            if (nextTickTime < ElapsedTime)
            {
                nextTickTime = ElapsedTime + TICK_INTERVAL;
                foreach (Character character in TeamBlue)
                {
                    updateCanvas = character.tick() || updateCanvas;
                }
                foreach (Character character in TeamRed)
                {
                    updateCanvas = character.tick() || updateCanvas;
                }
            }

            return updateCanvas;
        }
    }
}
