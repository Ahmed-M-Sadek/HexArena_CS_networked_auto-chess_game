using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ASU2019_NetworkedGameWorkshop.model.grid {
    public class Grid : GraphicsObject {
        private readonly int gridWidth, gridHeight;
        private readonly int startingX, startingY;
        private readonly controller.GameManager gameManager;
        private readonly Bitmap tilesBitmap;
        private readonly Bitmap tilesTransparentBitmap;

        public Tile[,] Tiles { get; }//temp public

        public int MaxSize => gridHeight * gridWidth;

        public bool Transparent { get; set; }

        public Grid(int gridWidth, int gridHeight, int startingX, int startingY, controller.GameManager gameManager) {
            this.gridWidth = gridWidth;
            this.gridHeight = gridHeight;
            this.startingX = startingX;
            this.startingY = startingY;
            this.gameManager = gameManager;

            Tiles = new Tile[gridWidth, gridHeight];
            for(int y = 0; y < gridHeight; y++) {
                for(int x = 0; x < gridWidth; x++) {
                    Tiles[x, y] = new Tile(x, y, startingX, startingY);
                }
            }

            tilesBitmap = createTilesBitmap(gridWidth, gridHeight, startingX, startingY, Tiles, false);
            tilesTransparentBitmap = createTilesBitmap(gridWidth, gridHeight, startingX, startingY, Tiles, true);
        }

        private static Bitmap createTilesBitmap(int gridWidth, int gridHeight, int startingX, int startingY, Tile[,] tiles, bool transparent)
        {
            Bitmap bitmap= new Bitmap(startingX + (int)((gridWidth + 1) * Tile.WIDTH),
                startingY + (int)(gridHeight * Tile.HEIGHT),
                PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(bitmap);

            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode  = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode      = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            foreach (Tile tile in tiles)
            {
                bool temp = tile.Transparent;
                tile.Transparent = transparent;
                tile.draw(graphics);
                tile.Transparent = temp;
            }
            return bitmap;
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


        //valid for  odd_r hexagons
        public List<Tile> getNeighbours(Tile tile, Tile[,] TilesClone) {
            List<Tile> neighbours = new List<Tile>();
            for(int x = -1; x <= 1; x++) {
                for(int y = -1; y <= 1; y++) {
                    if((x == -1 && y == -1) ||
                        (x == -1 && y == 1) ||
                        (x == 0 && y == 0))
                        continue;

                    int gridX, gridY;
                    //checks if even
                    if((tile.Y & 1) != 0) {
                        gridX = tile.X + x;
                        gridY = tile.Y + y;
                    } else {
                        gridX = tile.X - x;
                        gridY = tile.Y - y;
                    }

                    if(gridX >= 0 && gridX < gridWidth &&
                        gridY >= 0 && gridY < gridHeight) {
                        neighbours.Add(TilesClone[gridX, gridY]);
                    }
                }
            }
            return neighbours;
        }

        public override void draw(Graphics graphics) {
            graphics.DrawImage(Transparent ? tilesTransparentBitmap : tilesBitmap, 0, 0);
            if (gameManager.SelectedTile != null)
            {
                gameManager.SelectedTile.draw(graphics);
            }
        }

        public override void drawDebug(Graphics graphics)
        {
            foreach (Tile tile in Tiles)
            {
                tile.drawDebug(graphics);
            }
        }
    }
}
