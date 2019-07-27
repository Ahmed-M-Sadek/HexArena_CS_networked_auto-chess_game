using System.Drawing;

namespace ASU2019_NetworkedGameWorkshop.model
{
    public abstract class GraphicsObject
    {
        public abstract void draw(Graphics graphics);
        public abstract void drawDebug(Graphics graphics);
    }
}