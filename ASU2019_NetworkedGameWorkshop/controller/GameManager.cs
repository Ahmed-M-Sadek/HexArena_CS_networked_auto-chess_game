using ASU2019_NetworkedGameWorkshop.model.character;
using ASU2019_NetworkedGameWorkshop.model.character.types;
using ASU2019_NetworkedGameWorkshop.model.grid;
using ASU2019_NetworkedGameWorkshop.model.ui;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace ASU2019_NetworkedGameWorkshop.controller {
    public class GameManager {
        private enum GameStage { Buy, Fight }
        private const int GAMELOOP_INTERVAL = 50, TICK_INTERVAL = 1000;
        private const int GRID_HEIGHT = 6, GRID_WIDTH = 7;

        private readonly Grid grid;
        private readonly GameForm gameForm;
        private readonly Timer timer;
        private readonly StageTimer stageTimer;
        private readonly Stopwatch stopwatch;

        private Tile selectedTile;
        private long nextTickTime;
        private GameStage gameStage;
        private Dictionary<Character, Tile> charactersPrevPos;

        public long ElapsedTime {
            get {
                return stopwatch.ElapsedMilliseconds;
            }
        }
        public List<Character> TeamBlue { get; set; }//temp puplic setter
        public List<Character> TeamRed { get; set; }//temp puplic setter

        public GameManager(GameForm gameForm) {
            this.gameForm = gameForm;
            grid = new Grid(GRID_WIDTH, GRID_HEIGHT,
                (int) ((gameForm.Width - (Tile.WIDTH * GRID_WIDTH)) / 2),
                (int) ((gameForm.Height - (Tile.HEIGHT * GRID_HEIGHT)) / 2) + 30);//temp values

            TeamBlue = new List<Character>();
            TeamRed = new List<Character>();

            stopwatch = new Stopwatch();

            gameStage = GameStage.Buy;
            stageTimer = new StageTimer(this, switchStage);
            charactersPrevPos = new Dictionary<Character, Tile>();

            timer = new Timer {
                Interval = GAMELOOP_INTERVAL //Arbitrary: 20 ticks per sec
            };
            timer.Tick += new EventHandler(gameLoop);

            //Debugging 
            TeamRed.Add(new Character(grid, grid.Tiles[6, 5], Character.Teams.Red, CharacterTypePhysical.Archer, this));
            TeamBlue.Add(new Character(grid, grid.Tiles[4, 0], Character.Teams.Blue, CharacterTypePhysical.Archer, this));
            TeamBlue.Add(new Character(grid, grid.Tiles[0, 3], Character.Teams.Blue, CharacterTypePhysical.Archer, this));
        }

        private void switchStage() {
            if(gameStage == GameStage.Buy) {
                gameStage = GameStage.Fight;

                charactersPrevPos.Clear();
                foreach(Character character in TeamRed) {
                    charactersPrevPos.Add(character, character.CurrentTile);
                }
                foreach(Character character in TeamBlue) {
                    charactersPrevPos.Add(character, character.CurrentTile);
                }

                foreach(Tile tile in grid.Tiles) {
                    tile.Transparent = true;
                }

                if(selectedTile != null) {
                    selectedTile.Selected = false;
                    selectedTile = null;
                }
                stageTimer.resetTimer(StageTimer.StageTime.FIGHT_TIME);
            } else {
                gameStage = GameStage.Buy;

                foreach(Character character in TeamRed) {
                    character.CurrentTile.CurrentCharacter = null;
                }
                TeamRed.Clear();
                foreach(Character character in TeamBlue) {
                    character.CurrentTile.CurrentCharacter = null;
                }
                TeamBlue.Clear();

                foreach(KeyValuePair<Character, Tile> characterPrevPos in charactersPrevPos) {
                    if(characterPrevPos.Key.team == Character.Teams.Blue) {
                        TeamBlue.Add(characterPrevPos.Key);
                    } else {
                        TeamRed.Add(characterPrevPos.Key);
                    }
                    characterPrevPos.Value.CurrentCharacter = characterPrevPos.Key;
                    characterPrevPos.Key.reset();
                }
                foreach(Tile tile in grid.Tiles) {
                    tile.Transparent = false;
                }
                stageTimer.resetTimer(StageTimer.StageTime.BUY_TIME);
            }
            gameForm.Invalidate();
        }

        public void startTimer() {
            gameStart();
            timer.Start();
        }

        public void mouseClick(MouseEventArgs e) {
            if(gameStage == GameStage.Buy) {
                if(e.Button == MouseButtons.Right) {
                    selectedTile.Selected = false;
                    selectedTile = null;
                    gameForm.Invalidate();
                } else if(e.Button == MouseButtons.Left) {
                    tileSelection(e.X, e.Y);
                }
            }
        }

        private void tileSelection(int x, int y) {
            Tile tile = grid.getSelectedHexagon(x, y);
            if(tile != null) {
                if(selectedTile == tile) {
                    selectedTile.Selected = false;
                    selectedTile = null;
                } else if(selectedTile == null) {
                    selectedTile = tile;
                    selectedTile.Selected = true;
                } else {
                    selectedTile.Selected = false;
                    Character temp = selectedTile.CurrentCharacter;
                    selectedTile.CurrentCharacter = tile.CurrentCharacter;
                    tile.CurrentCharacter = temp;
                    selectedTile = null;
                }

                gameForm.Invalidate();
            }
        }
        public void updatePaint(PaintEventArgs e) {
            grid.draw(e.Graphics);

            TeamBlue.ForEach(character => character.draw(e.Graphics));
            TeamRed.ForEach(character => character.draw(e.Graphics));

            stageTimer.draw(e.Graphics);
        }

        private void gameStart() {
            stopwatch.Start();
            stageTimer.resetTimer(StageTimer.StageTime.DEBUGGIN_TIME);//Debugging
            //stageTimer.resetTimer(StageTimer.StageTime.BUY_TIME);
        }

        private void gameLoop(object sender, EventArgs e) {
            bool updateCanvas = stageTimer.update();

            if(gameStage == GameStage.Buy) {
                updateCanvas = stageUpdateBuy() || updateCanvas;
            } else {
                updateCanvas = stageUpdateFight() || updateCanvas;
            }

            if(updateCanvas)
                gameForm.Invalidate();
        }

        private bool stageUpdateBuy() {
            bool updateCanvas = false;
            return updateCanvas;
        }

        private bool stageUpdateFight() {
            if(TeamBlue.Count == 0 || TeamRed.Count == 0) {
                stageTimer.endTimer();
                return false;
            }

            bool updateCanvas = false;
            bool predicate(Character character) {
                if(character.IsDead) {
                    updateCanvas = true;
                    return false;
                }
                return true;
            }
            foreach(Character character in TeamBlue) {
                updateCanvas = character.update() || updateCanvas;
            }
            TeamBlue = TeamBlue.Where(predicate).ToList();
            foreach(Character character in TeamRed) {
                updateCanvas = character.update() || updateCanvas;
            }
            TeamRed = TeamRed.Where(predicate).ToList();

            if(nextTickTime < ElapsedTime) {
                nextTickTime = ElapsedTime + TICK_INTERVAL;
                foreach(Character character in TeamBlue) {
                    updateCanvas = character.tick() || updateCanvas;
                }
                foreach(Character character in TeamRed) {
                    updateCanvas = character.tick() || updateCanvas;
                }
            }

            return updateCanvas;
        }
    }
}
