using System;
using System.Drawing;

namespace ASU2019_NetworkedGameWorkshop.model.grid {
    class Grid : GraphicsObject {
        private const float halfWidth = (Tile.WIDTH / 2);//in tile ?
        private const float hexC = halfWidth * 0.57735026919f;

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

            float tileHeight = (Tile.HEIGHT - hexC);

            int row = (int) (y / tileHeight);

            bool rowIsOdd = row % 2 == 1;

            int column = rowIsOdd ?
                (int) ((x - halfWidth) / Tile.WIDTH) :
                column = (int) (x / Tile.WIDTH);

            double relY = y - (row * tileHeight);
            double relX = rowIsOdd ?
                (x - (column * Tile.WIDTH)) - halfWidth :
                x - (column * Tile.WIDTH);

            float m = (hexC / halfWidth);

            if(relY < (-m * relX) + hexC) {
                row--;
                if(!rowIsOdd)
                    column--;
            } else if(relY < m * relX - hexC) {
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
