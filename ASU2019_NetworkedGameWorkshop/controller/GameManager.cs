using ASU2019_NetworkedGameWorkshop.model.character;
using ASU2019_NetworkedGameWorkshop.model.character.types;
using ASU2019_NetworkedGameWorkshop.model.grid;
using ASU2019_NetworkedGameWorkshop.model.Shop;
using System;
using System.Linq;
using System.Windows.Forms;

namespace ASU2019_NetworkedGameWorkshop.controller {
    public class GameManager {
        private const int GAMELOOP_INTERVAL = 50, TICK_INTERVAL = 1000;
        private const int GRID_HEIGHT = 6, GRID_WIDTH = 7;

        private readonly Grid grid;
        private readonly GameForm gameForm;
        private readonly Timer timer;

        private Tile selectedTile;
        public Tile SelectedTile {
            get {
                return selectedTile;
            }
        }
        private long nextTickTime;
        private Shop shop;

        public long ElapsedTime { get; private set; }

        public GameManager(GameForm gameForm) {
            this.gameForm = gameForm;
            grid = new Grid(GRID_WIDTH, GRID_HEIGHT,
                (int)((gameForm.Width - (Tile.WIDTH * GRID_WIDTH)) / 2),
                (int)((gameForm.Height - (Tile.HEIGHT * GRID_HEIGHT)) / 2));//temp values

            timer = new Timer();
            timer.Interval = GAMELOOP_INTERVAL; //Arbitrary: 20 ticks per sec
            timer.Tick += new EventHandler(gameLoop);

            //Debugging 
            new Character(grid, grid.Tiles[6, 5], Character.Teams.Red, CharacterTypeRanged.Archer, this);
            new Character(grid, grid.Tiles[0, 3], Character.Teams.Blue, CharacterTypeRanged.Archer, this);
            new Character(grid, grid.Tiles[4, 0], Character.Teams.Blue, CharacterTypeRanged.Archer, this);
            shop = new Shop(gameForm);
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
            if (tile != null) {
                Console.WriteLine("Clicked Tile: ({0}, {1})", tile.X, tile.Y);//Debugging

                if (selectedTile != null) {
                    selectedTile.Selected = false;
                }
                if (selectedTile == tile) {
                    selectedTile = null;
                } else {
                    selectedTile = tile;
                    selectedTile.Selected = true;
                }

                tile.Walkable = false;//Debugging

                gameForm.Invalidate();
            }
            if(selectedTile != null) {
                shop.updateShop(selectedTile.CurrentCharacter);
            }
            
        }

        private void gameLoop(object sender, EventArgs e) {
            ElapsedTime += GAMELOOP_INTERVAL;

            bool updateCanvas = false;

            foreach (Character character in grid.TeamBlue) {
                bool temp = character.update();
                updateCanvas = updateCanvas || temp;
            }
            grid.TeamBlue = grid.TeamBlue.Where(character => !character.IsDead).ToList();
            foreach (Character character in grid.TeamRed) {
                bool temp = character.update();
                updateCanvas = updateCanvas || temp;
            }
            grid.TeamRed = grid.TeamRed.Where(character => !character.IsDead).ToList();



            if (nextTickTime < ElapsedTime) {
                nextTickTime += TICK_INTERVAL;
                foreach (Character character in grid.TeamBlue) {
                    updateCanvas = character.tick() || updateCanvas;
                }
                foreach (Character character in grid.TeamRed) {
                    updateCanvas = character.tick() || updateCanvas;
                }
            }
            if (updateCanvas)
                gameForm.Invalidate();
        }
    }
}
