using System;
using System.Drawing;

namespace ASU2019_NetworkedGameWorkshop.model.grid {
    class Grid : GraphicsObject {
        private readonly int gridWidth, gridHeight;
        private readonly int startingX, startingY;

        private Tile[,] tiles;
        public Grid(int gridWidth, int gridHeight, int startingX, int startingY) {
            if(gridWidth < 0 ||
                gridHeight < 0 ||
                startingX < 0 ||
                startingY < 0)
                throw new ArgumentOutOfRangeException("Negative Input");//not descriptive 

            this.gridWidth = gridWidth;
            this.gridHeight = gridHeight;
            this.startingX = startingX;
            this.startingY = startingY;

            tiles = new Tile[gridWidth, gridHeight];
            for(int y = 0; y < gridHeight; y++) {
                for(int x = 0; x < gridWidth; x++) {
                    tiles[x, y] = new Tile(
                        startingX + (y % 2 == 0 ? (x * 43 * 2) : (x * 43 * 2) + 43),
                        startingY + (y * 50 * 2 * 3 / 4));
                }
            }
        }

        public override void draw(Graphics graphics) {
            foreach(Tile tile in tiles) {
                tile.draw(graphics);
            }
        }
    }
}
