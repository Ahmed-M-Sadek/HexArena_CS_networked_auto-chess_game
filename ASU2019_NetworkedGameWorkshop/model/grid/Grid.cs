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
                    tiles[x, y] = new Tile(x, y, startingX, startingY);
                }
            }
        }

        internal Tile getSelectedHexagon(int x, int y) {
            x -= startingX;
            y -= startingY;

            const float halfwidth = (Tile.WIDTH / 2); //move to the class
            const float c = (float) (halfwidth * 0.57735026919);
            float tileHeight = (Tile.HEIGHT - c);

            int row = (int) (y / tileHeight);

            bool rowIsOdd = row % 2 == 1;

            int column = rowIsOdd ?
                (int) ((x - halfwidth) / Tile.WIDTH) :
                column = (int) (x / Tile.WIDTH);

            double relY = y - (row * tileHeight);
            double relX = rowIsOdd ?
                (x - (column * Tile.WIDTH)) - halfwidth :
                x - (column * Tile.WIDTH);

            float m = (c / halfwidth);

            if(relY < (-m * relX) + c) {
                row--;
                if(!rowIsOdd)
                    column--;
            } else if(relY < m * relX - c) {
                row--;
                if(rowIsOdd)
                    column++;
            }
            if(column < gridWidth
                && row < gridHeight
                && column > -1
                && row > -1)
                return tiles[column, row];
            return null;
        }

        public override void draw(Graphics graphics) {
            foreach(Tile tile in tiles) {
                tile.draw(graphics);
            }
        }
    }
}
