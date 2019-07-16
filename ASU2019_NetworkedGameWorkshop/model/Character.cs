using System.Drawing;

namespace ASU2019_NetworkedGameWorkshop.model {
    class Character : GraphicsObject {
        public float X { get; set; }
        public float Y { get; set; }



        public override void draw(Graphics graphics) {
            graphics.FillRectangle(Brushes.HotPink, X, Y, 25, 25);
        }
    }
}
