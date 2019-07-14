using System.Drawing;

namespace ASU2019_NetworkedGameWorkshop.model.grid {
    class Tile : GraphicsObject {
        private const float height = 100f, width = 86.6f; //todo

        private static readonly Image image = Image.FromFile("../../assets/sprites/tiles/Tile.png");//todo path

        private float x, y;//id
        private Character currebtCharacter = null;

        public Tile(float x, float y) {
            this.x = x;
            this.y = y;
        }

        public override void draw(Graphics graphics) {
            graphics.DrawImage(image, x, y, width, height);
        }
    }
}
