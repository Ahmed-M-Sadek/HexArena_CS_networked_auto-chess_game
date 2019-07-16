using System;
using System.Collections.Generic;
using System.Drawing;

namespace ASU2019_NetworkedGameWorkshop.model.grid {
    internal class Tile : GraphicsObject , IHeapItem<Tile>, ICloneable{
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

        private float posX, posY;
        //todo remove character from here
        public Character CurrentCharacter { get; set; }

        public bool Walkable { get; set; }
        public int Gcost { get; set; }
        public int Hcost { get; set; }
        public Tile Parent{ get; set; }
        public int Fcost
        {
            get
            {
                return Gcost + Hcost;
            }
        }

        //temp
        public bool InPath = false;

        public int HeapIndex { get ; set ; }

        public Tile(int x, int y, float startingX, float startingY) {
            X = x;
            Y = y;

            posX = startingX + x * WIDTH + (y % 2 == 0 ? 0 : 43);
            posY = startingY + y * (HEIGHT - HEX_C);

            Walkable = true;
        }

        public override void draw(Graphics graphics) {
            graphics.DrawImage(Selected ? imageSelected : image, posX, posY, WIDTH, HEIGHT);

            if(CurrentCharacter != null) {
                CurrentCharacter.X = posX + HALF_WIDTH / 2;
                CurrentCharacter.Y = posY + HEX_C * 3 / 2;
                CurrentCharacter.draw(graphics);
            }

            //debug
            graphics.DrawString(X + ", " + Y, new Font("Roboto", 14f),
                (InPath) ? Brushes.Blue : (Walkable) ? Brushes.Purple : Brushes.Red, 
                new PointF(posX + 30, posY + 40));
        }

        public int CompareTo(Tile other)
        {
            int compare = Fcost.CompareTo(other.Fcost);
            if (compare == 0)
            {
                compare = Hcost.CompareTo(other.Hcost);
            }
            return -compare;
        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
