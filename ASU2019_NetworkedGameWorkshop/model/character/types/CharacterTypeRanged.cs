using System.Collections.Generic;

namespace ASU2019_NetworkedGameWorkshop.model.character.types {
    public class CharacterTypeRanged : CharacterType {
        public static readonly CharacterTypeRanged Archer = new CharacterTypeRanged(
            50, 50, 2, 100, 100);

        public CharacterTypeRanged(int width, int height, int range, int baseHP, int maxChargePoints)
            : base(width, height, range, baseHP, maxChargePoints) {
        }

        public static IEnumerable<CharacterTypeRanged> Values {
            get {
                yield return Archer;
            }
        }
    }
}
