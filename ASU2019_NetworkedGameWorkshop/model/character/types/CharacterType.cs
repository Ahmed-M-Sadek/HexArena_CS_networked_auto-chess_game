using ASU2019_NetworkedGameWorkshop.model.grid;

namespace ASU2019_NetworkedGameWorkshop.model.character.types {
    public abstract class CharacterType {
        public int Width { get; private set; }
        public int WidthHalf { get; private set; }

        public int Height { get; private set; }
        public int HeightHalf { get; private set; }

        public int Range { get; private set; }

        protected CharacterType(int width, int height, int range) {
            Width = width;
            WidthHalf = width / 2;
            Height = height;
            HeightHalf = height / 2;
            Range = range;
        }

        public abstract void attack(Tile tile);

    }
}
