using System.Drawing;

namespace ASU2019_NetworkedGameWorkshop.model.grid {
    class Tile : GraphicsObject {
        public const float HEIGHT = 100f, WIDTH = 86.6f; //todo
        public const float HALF_WIDTH = WIDTH / 2;
        public const float HEX_C = HALF_WIDTH * 0.57735026919f;
        public const float HEX_HEIGHT = HEIGHT - HEX_C;
        public const float HEX_M = HEX_C / HALF_WIDTH;

        private static readonly Image image = Image.FromFile("../../assets/sprites/tiles/Tile.png");//todo path
        private static readonly Image imageSelected = Image.FromFile("../../assets/sprites/tiles/Tile_Selected.png");//todo path
        public int X { get; private set; }
        public int Y { get; private set; }
        public bool Selected { get; set; }

        private readonly float posX, posY;

        private Character currebtCharacter;

        public Tile(int x, int y, float startingX, float startingY) {
            X = x;
            Y = y;

            posX = startingX + x * WIDTH + (y % 2 == 0 ? 0 : 43);
            posY = startingY + y * (HEIGHT - HEX_C);
        }

        public override void draw(Graphics graphics) {
            graphics.DrawImage(Selected ? imageSelected : image, posX, posY, WIDTH, HEIGHT);

            //debug
            graphics.DrawString(X + ", " + Y,
                new Font("Roboto", 18f), Brushes.Purple, new PointF(posX + HALF_WIDTH / 2, posY + HEX_C*3/2));
        }
    }
}
