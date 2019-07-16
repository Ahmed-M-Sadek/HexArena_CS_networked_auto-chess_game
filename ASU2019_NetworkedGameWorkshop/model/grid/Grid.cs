using System.Drawing;

namespace ASU2019_NetworkedGameWorkshop.model.grid {
    class Grid : GraphicsObject {
        private readonly int gridWidth, gridHeight;
        private readonly int startingX, startingY;
        public Tile[,] Tiles { get;}//temp public

        public Grid(int gridWidth, int gridHeight, int startingX, int startingY) {
            this.gridWidth = gridWidth;
            this.gridHeight = gridHeight;
            this.startingX = startingX;
            this.startingY = startingY;

            Tiles = new Tile[gridWidth, gridHeight];
            for(int y = 0; y < gridHeight; y++) {
                for(int x = 0; x < gridWidth; x++) {
                    Tiles[x, y] = new Tile(x, y, startingX, startingY);
                }
            }
        }

        //Credits: https://stackoverflow.com/questions/7705228/hexagonal-grids-how-do-you-find-which-hexagon-a-point-is-in
        public Tile getSelectedHexagon(int x, int y) {
            x -= startingX;
            y -= startingY;

            int row = (int) (y / Tile.HEX_HEIGHT);

            bool rowIsOdd = row % 2 == 1;

            int column = (int) ((x - (rowIsOdd ? Tile.HALF_WIDTH : 0)) / Tile.WIDTH);

            double relY = y - (row * Tile.HEX_HEIGHT);
            double relX = x - (column * Tile.WIDTH) - (rowIsOdd ? Tile.HALF_WIDTH : 0);

            if(relY < (-Tile.HEX_M * relX) + Tile.HEX_C) {
                row--;
                if(!rowIsOdd)
                    column--;
            } else if(relY < (Tile.HEX_M * relX) - Tile.HEX_C) {
                row--;
                if(rowIsOdd)
                    column++;
            }

            if(column < gridWidth
                && row < gridHeight
                && column > -1
                && row > -1)
                return Tiles[column, row];
            return null;
        }

        public override void draw(Graphics graphics) {
            foreach(Tile tile in Tiles) {
                tile.draw(graphics);
            }
        }
    }
}
