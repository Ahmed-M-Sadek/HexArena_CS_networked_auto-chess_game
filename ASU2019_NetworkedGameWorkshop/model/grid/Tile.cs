using ASU2019_NetworkedGameWorkshop.model.character;
using System;
using System.Drawing;
using ASU2019_NetworkedGameWorkshop.Properties;

namespace ASU2019_NetworkedGameWorkshop.model.grid {
    public class Tile : GraphicsObject, IHeapItem<Tile>, ICloneable {
        public const float HEIGHT = 100f * 1.3f, WIDTH = 86.6f * 1.3f; //todo
        public const float HALF_HEIGHT = HEIGHT / 2f, HALF_WIDTH = WIDTH / 2f;
        public const float HEX_C = HALF_WIDTH * 0.57735026919f,
            HEX_HEIGHT = HEIGHT - HEX_C,
            HEX_M = HEX_C / HALF_WIDTH;

        private static readonly Image image = Resources.Tile,
            imageTransparent = Resources.Tile_Transparent,
            imageSelected = Resources.Tile_Selected;//todo path

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
                if(value != null) {
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

        public Tile(int x, int y, float startingX, float startingY) {
            X = x;
            Y = y;

            posX = startingX + x * WIDTH + (y % 2 == 0 ? 0 : HALF_WIDTH);
            posY = startingY + y * (HEIGHT - HEX_C);

            centerX = posX + HALF_WIDTH;
            centerY = posY + HEIGHT / 2;

            Walkable = true;
        }

        public override void draw(Graphics graphics) {
            if(Transparent) {
                graphics.DrawImage(imageTransparent, posX, posY, WIDTH, HEIGHT);
            } else {
                graphics.DrawImage(Selected ? imageSelected : image, posX, posY, WIDTH, HEIGHT);
            }

            //debug
            graphics.DrawString(this.ToString(), new Font("Roboto", 14f),
                (!Walkable) ? Brushes.Red : Brushes.Purple,
                centerX, centerY);
        }

        public int CompareTo(Tile other) {
            int compare = Fcost.CompareTo(other.Fcost);
            if(compare == 0) {
                compare = Hcost.CompareTo(other.Hcost);
            }
            return -compare;
        }

        public object Clone() {
            return MemberwiseClone();
        }

        public override bool Equals(object obj) {
            return (obj is Tile item) && item.X == X && item.Y == Y;
        }

        public override string ToString() {
            return string.Format("({0}, {1})", X, Y);
        }
    }
}
