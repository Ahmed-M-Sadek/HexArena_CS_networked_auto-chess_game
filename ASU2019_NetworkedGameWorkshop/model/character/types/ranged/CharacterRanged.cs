namespace ASU2019_NetworkedGameWorkshop.model.character.types.ranged {
    abstract class CharacterRanged : CharacterType {
        protected CharacterRanged(int width, int height, int range)
            : base(width, height, range) {
        }
    }
}
