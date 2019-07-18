using ASU2019_NetworkedGameWorkshop.model.grid;
using System;

namespace ASU2019_NetworkedGameWorkshop.model.character.types.ranged {
    class Archer : CharacterRanged {

        public static readonly Archer archer = new Archer(50, 50, 1);

        public Archer(int width, int height, int range)
            : base(width, height, range) {
        }

        public override void attack(Tile tile) {
            Console.WriteLine("Attacking: " + tile);//Debug
        }
    }
}
