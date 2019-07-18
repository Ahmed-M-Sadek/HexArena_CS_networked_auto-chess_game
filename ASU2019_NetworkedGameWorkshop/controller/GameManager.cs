using ASU2019_NetworkedGameWorkshop.model.character;
using ASU2019_NetworkedGameWorkshop.model.character.types.ranged;
using ASU2019_NetworkedGameWorkshop.model.grid;
using System;
using System.Windows.Forms;

namespace ASU2019_NetworkedGameWorkshop.controller {
    class GameManager {
        private const int GAMELOOP_INTERVAL = 50, TICK_INTERVAL = 1500;
        private const int GRID_HEIGHT = 6, GRID_WIDTH = 7;

        private readonly Grid grid;
        private readonly GameForm gameForm;
        private readonly Timer timer;

        private Tile selectedTile;
        private long nextTickTime;

        public long ElapsedTime { get; private set; }

        public GameManager(GameForm gameForm) {
            this.gameForm = gameForm;
            grid = new Grid(GRID_WIDTH, GRID_HEIGHT,
                (int) ((gameForm.Width - (Tile.WIDTH * GRID_WIDTH)) / 2),
                (int) ((gameForm.Height - (Tile.HEIGHT * GRID_HEIGHT)) / 2));//temp values

            timer = new Timer();
            timer.Interval = GAMELOOP_INTERVAL; //Arbitrary: 20 ticks per sec
            timer.Tick += new EventHandler(gameLoop);

            //Debugging 
            new Character(grid, grid.Tiles[6, 5], Character.Teams.Red, Archer.archer);
            new Character(grid, grid.Tiles[0, 3], Character.Teams.Blue, Archer.archer);
            new Character(grid, grid.Tiles[4, 0], Character.Teams.Blue, Archer.archer);
        }

        public void startTimer() {
            timer.Start();
        }

        public void updatePaint(PaintEventArgs e) {
            grid.draw(e.Graphics);

            grid.TeamBlue.ForEach(character => character.draw(e.Graphics));
            grid.TeamRed.ForEach(character => character.draw(e.Graphics));
        }

        public void mouseClick(MouseEventArgs e) {
            tileSelection(e.X, e.Y);
        }

        private void tileSelection(int x, int y) {
            Tile tile = grid.getSelectedHexagon(x, y);
            if(tile != null) {
                Console.WriteLine("Clicked Tile: ({0}, {1})", tile.X, tile.Y);//Debugging

                if(selectedTile != null) {
                    selectedTile.Selected = false;
                }
                if(selectedTile == tile) {
                    selectedTile = null;
                } else {
                    selectedTile = tile;
                    selectedTile.Selected = true;
                }

                tile.Walkable = false;//Debugging

                gameForm.Invalidate();
            }
        }

        private void gameLoop(object sender, EventArgs e) {
            ElapsedTime += GAMELOOP_INTERVAL;

            bool updateCanvas = false;

            foreach(Character character in grid.TeamBlue) {
                updateCanvas = updateCanvas || character.update();
            }
            foreach(Character character in grid.TeamRed) {
                updateCanvas = updateCanvas || character.update();
            }

            if(nextTickTime < ElapsedTime) {
                nextTickTime += TICK_INTERVAL;
                foreach(Character character in grid.TeamBlue) {
                    updateCanvas = character.tick() || updateCanvas;
                }
                foreach(Character character in grid.TeamRed) {
                    updateCanvas = character.tick() || updateCanvas;
                }
            }
            if(updateCanvas)
                gameForm.Invalidate();
        }
    }
}
