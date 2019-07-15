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

        internal Tuple<int, int> mouseClick(int x, int y) {
            //foreach(Tile tile in tiles) {
            //    if(tile.contains(x, y)) {
            //        return Tuple.Create(tile.X, tile.Y);
            //    }
            //}
            //return null;

            return getSelectedHexagon(x, y);
        }

        private Tuple<int, int> getSelectedHexagon(int x, int y) {
            const float halfwidth = (Tile.WIDTH / 2);
            float c = (float) (halfwidth * Math.Tan(0.523599));
            int row = (int) (y / (Tile.HEIGHT - c));

            bool rowIsOdd = row % 2 == 1;

            int column = rowIsOdd ? (int) ((x - halfwidth) / Tile.WIDTH) : column = (int) (x / Tile.WIDTH);

            double relY = y - (row * (Tile.HEIGHT - c));
            double relX = rowIsOdd ? (x - (column * Tile.WIDTH)) - halfwidth : x - (column * Tile.WIDTH);

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

            return Tuple.Create(column, row);
        }

        public override void draw(Graphics graphics) {
            foreach(Tile tile in tiles) {
                tile.draw(graphics);
            }
        }
    }
}
