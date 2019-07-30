using ASU2019_NetworkedGameWorkshop.model.character;
using ASU2019_NetworkedGameWorkshop.Properties;
using System;
using System.Drawing;

namespace ASU2019_NetworkedGameWorkshop.model.grid {
    public class Tile : GraphicsObject, IHeapItem<Tile>, ICloneable {
        public const float HEIGHT = 100f * 1.3f, WIDTH = 86.6f * 1.3f; //todo
        public const float HALF_HEIGHT = HEIGHT / 2f, HALF_WIDTH = WIDTH / 2f;
        public const float HEX_C = HALF_WIDTH * 0.57735026919f,
            HEX_HEIGHT = HEIGHT - HEX_C,
            HEX_M = HEX_C / HALF_WIDTH;

        private static readonly Font DEBUG_FONT = new Font("Roboto", 14f);

        public readonly float centerX, centerY;

        private readonly float posX, posY;

        public int X { get; private set; }
        public int Y { get; private set; }
        public bool Selected { get; set; }

        //todo remove character from here
        private Character currentCharacter;
        public Character CurrentCharacter {
            get {
                return currentCharacter;
            }
            set {
                currentCharacter = value;
                if (value != null) {
                    value.CurrentTile = this;
                    Walkable = false;
                } else {
                    Walkable = true;
                }
            }
        }

        public bool Walkable { get; set; }
        public int Gcost { get; set; }
        public int Hcost { get; set; }
        public Tile Parent { get; set; }
        public int Fcost {
            get {
                return Gcost + Hcost;
            }
        }

        //temp
        public bool InPath = false;

        public int HeapIndex { get; set; }
        public bool Transparent { get; set; }

        public Tile(int x, int y, int startingX, int startingY) {
            X = x;
            Y = y;

            posX = startingX + x * WIDTH + (y % 2 == 0 ? 0 : HALF_WIDTH);
            posY = startingY + y * (HEIGHT - HEX_C);

            centerX = posX + HALF_WIDTH;
            centerY = posY + HEIGHT / 2;

            Walkable = true;
        }


        /// <summary>
        /// Draws a string containing the tile's coordinates on the tile.
        /// </summary>
        /// <param name="graphics">graphics object to draw on</param>
        public override void drawDebug(Graphics graphics) {
            graphics.DrawString(ToString(), DEBUG_FONT,
                Walkable ? Brushes.Purple : Brushes.Orange,
                centerX, centerY);
        }

        public override void draw(Graphics graphics) {
            if (Transparent) {
                graphics.DrawImage(Resources.Tile_Transparent, posX, posY, WIDTH, HEIGHT);
            } else {
                graphics.DrawImage(Selected ? Resources.Tile_Selected : Resources.Tile, posX, posY, WIDTH, HEIGHT);
            }
        }

        public int CompareTo(Tile other) {
            int compare = Fcost.CompareTo(other.Fcost);
            if (compare == 0) {
                compare = Hcost.CompareTo(other.Hcost);
            }
            return -compare;
        }


        /// <summary>
        /// MemberwiseClone
        /// </summary>
        /// <returns></returns>
        public object Clone() {
            return MemberwiseClone();
        }

        /// <summary>
        /// checks equality of the X and Y of the current and given objects.
        /// </summary>
        /// <param name="obj">The object to check agnist.</param>
        /// <returns>returns true if obj is a tile and has the same X and Y as the current tile. false otherwise.</returns>
        public override bool Equals(object obj) {
            return (obj is Tile item) && item.X == X && item.Y == Y;
        }

        /// <summary>
        /// The string representation of the Tile.
        /// </summary>
        /// <returns>returns a formatted string containing the X and Y of the tile.</returns>
        public override string ToString() {
            return string.Format("({0}, {1})", X, Y);
        }
    }
}
