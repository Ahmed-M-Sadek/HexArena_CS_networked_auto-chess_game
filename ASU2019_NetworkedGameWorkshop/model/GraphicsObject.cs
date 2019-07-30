using System.Drawing;

namespace ASU2019_NetworkedGameWorkshop.model {
    /// <summary>
    /// Base class for all visual visible classes.
    /// </summary>
    public abstract class GraphicsObject {
        public abstract void draw(Graphics graphics);
        public abstract void drawDebug(Graphics graphics);
    }
}