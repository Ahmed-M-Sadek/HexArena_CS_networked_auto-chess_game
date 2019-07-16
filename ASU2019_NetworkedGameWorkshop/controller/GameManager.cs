using ASU2019_NetworkedGameWorkshop.model;
using ASU2019_NetworkedGameWorkshop.model.grid;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ASU2019_NetworkedGameWorkshop.controller {
    class GameManager {
        private const int GAMELOOP_INTERVAL = 50;
        private const int TICK_INTERVAL = 500;
        private readonly Grid grid;
        private readonly GameForm gameForm;
        private readonly List<Character> CharacterList;
        private readonly Timer timer;
        private Tile selectedTile;
        private long nextTickTime;

        public long ElapsedTime { get; private set; }

        public GameManager(GameForm gameForm) {
            grid = new Grid(10, 8, 10, 10);//temp values
            this.gameForm = gameForm;

            timer = new Timer();
            timer.Interval = GAMELOOP_INTERVAL; //Arbitrary: 20 ticks per sec
            timer.Tick += new EventHandler(gameLoop);

            Character blue1 = new Character(grid, grid.Tiles[0, 5], Character.Teams.Blue);
            Character red1 = new Character(grid, grid.Tiles[2, 6], Character.Teams.Red);

            //Debugging 
            //todo remove current char from tile
            grid.Tiles[0, 5].CurrentCharacter = blue1;
            grid.Tiles[2, 6].CurrentCharacter = red1;
            //grid.Tiles[4, 1].CurrentCharacter = new Character(grid, grid.Tiles[4, 1]);
        }
        public void startTimer() {
            timer.Start();
        }

        public void updatePaint(PaintEventArgs e) {
            grid.draw(e.Graphics);
        }

        internal void mouseClick(MouseEventArgs e) {
            Tile tile = grid.getSelectedHexagon(e.X, e.Y);
            if(tile != null) {
                Console.WriteLine("Clicked Tile: ({0}, {1})", tile.X, tile.Y);//Debugging

                if(selectedTile == null) {
                    tile.Selected = true;
                    selectedTile = tile;
                } else {
                    if(tile.CurrentCharacter == null
                        && selectedTile.CurrentCharacter != null) {
                        selectedTile.CurrentCharacter.ToMoveTo = tile;
                    }

                    selectedTile.Selected = false;
                    selectedTile = null;
                }
                
                tile.Walkable = false;
                //if (grid.path != null && grid.path.Contains(tile))
                //{
                //    grid.findPath();
                //}
                
                gameForm.Invalidate();
            }
        }

        private void gameLoop(object sender, EventArgs e) {
            ElapsedTime += GAMELOOP_INTERVAL;

            if(nextTickTime < ElapsedTime) {
                nextTickTime += TICK_INTERVAL;
                Console.WriteLine("Tick " + ElapsedTime);
                bool updateCanvas = false;
                foreach(Character character in CharacterList) {
                    updateCanvas = character.tick();
                }
                if(updateCanvas)
                    gameForm.Invalidate();
            }
        }
    }
}
