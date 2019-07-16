using ASU2019_NetworkedGameWorkshop.model.grid;
using System.Drawing;

namespace ASU2019_NetworkedGameWorkshop.model {
    class Character : GraphicsObject {
        public float X { get; set; }
        public float Y { get; set; }

        public Tile CurrentTile { get; set; }
        public Tile ToMoveTo { get; set; }

        public override void draw(Graphics graphics) {
            graphics.FillRectangle(Brushes.HotPink, X, Y, 25, 25);
        }

        public bool tick() {
            if(ToMoveTo != null) {
                X = ToMoveTo.X;
                Y = ToMoveTo.Y;

                CurrentTile.CurrentCharacter = null;
                ToMoveTo.CurrentCharacter = this;
                ToMoveTo = null;

                return true;
            }
            return false;
        }
    }
}
