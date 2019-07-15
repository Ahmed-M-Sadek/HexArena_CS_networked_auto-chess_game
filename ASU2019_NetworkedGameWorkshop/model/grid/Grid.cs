using System;
using System.Collections.Generic;
using System.Drawing;

namespace ASU2019_NetworkedGameWorkshop.model.grid {
    class Grid : GraphicsObject {
        private readonly int gridWidth, gridHeight;
        private readonly int startingX, startingY;
        private Tile[,] tiles;

        public Tile[,] Tiles { get; set; }

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

            Console.WriteLine(PathFinding.getDistance(tiles[0, 0], tiles[0, 1]));
            Console.WriteLine(PathFinding.getDistance(tiles[0, 0], tiles[1, 2]));
            Console.WriteLine(PathFinding.getDistance(tiles[0, 0], tiles[2, 1]));
        }

        internal Tuple<int, int> mouseClick(int x, int y) {
            foreach(Tile tile in tiles) {
                if(tile.contains(x, y)) {
                    return Tuple.Create(tile.X, tile.Y);
                }
            }
            return null;
        }

        public override void draw(Graphics graphics) {
            foreach(Tile tile in tiles) {
                tile.draw(graphics);
            }
        }

        public List<Tile> getNeighbours(Tile tile)
        {
            List<Tile> neighbours = new List<Tile>();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == y)
                        continue;

                    int gridX = tile.X + x; 
                    int gridY = tile.Y + y; 

                    if ((gridX >= 0 && gridX < gridHeight) || 
                        (gridY >= 0 && gridY < gridWidth))
                    {
                        neighbours.Add(Tiles[gridX, gridY]);
                    }
                }
            }
            return neighbours;
        }
    }
}
