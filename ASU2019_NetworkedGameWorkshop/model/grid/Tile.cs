using System;
using System.Collections.Generic;
using System.Drawing;

namespace ASU2019_NetworkedGameWorkshop.model.grid {
    class Tile : GraphicsObject {
        public const float HEIGHT = 100f, WIDTH = 86.6f; //todo

        private static readonly Image image = Image.FromFile("../../assets/sprites/tiles/Tile.png");//todo path
        private static readonly Image imageSelected = Image.FromFile("../../assets/sprites/tiles/Tile_Selected.png");//todo path
        public int X { get; private set; }
        public int Y { get; private set; }
        public bool Selected { get; set; }

        private float posX, posY;
        private Character currentCharacter = null;

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

        public Tile(int x, int y, float startingX, float startingY) {
            this.X = x;
            this.Y = y;

            this.posX = startingX + (y % 2 == 0 ? (x * 43 * 2) : (x * 43 * 2) + 43);
            this.posY = startingY + (y * 50 * 2 * 3 / 4);

            Walkable = true;
        }

        public override void draw(Graphics graphics) {
            graphics.DrawImage(Selected ? imageSelected : image, posX, posY, WIDTH, HEIGHT);

            //debug
            graphics.DrawString(X + ", " + Y, new Font("Roboto", 14f),Brushes.Purple , new PointF(posX + 30, posY + 40));
        }



        internal bool contains(int x, int y) {
            throw new NotImplementedException();

        }

        internal void draw2(Graphics graphics)
        {
            graphics.DrawImage(image, posX, posY, width, height);
            //debug
            graphics.DrawString(X + ", " + Y, new Font("Roboto", 14f), Brushes.Blue, new PointF(posX + 30, posY + 40));
        }

    }
}
