using System.Collections.Generic;

namespace ASU2019_NetworkedGameWorkshop.model.character.types {
    public class CharacterTypeRanged : CharacterType {
        public static readonly CharacterTypeRanged[] Archer = new CharacterTypeRanged[]{
            new CharacterTypeRanged(
                500,
                50, 100,
                3,
                60, 0.7f,
                25, 20),
            new CharacterTypeRanged(
                500,
                50, 100,
                3,
                60, 0.7f,
                25, 20),
            new CharacterTypeRanged(
                500,
                50, 100,
                3,
                60, 0.7f,
                25, 20),
        };

        public CharacterTypeRanged(
            int healthPoints,
            int charge, int chargeMax,
            int range,
            int attackDamage, float attackSpeed,
            int armor, int magicResist)
            : base(healthPoints, charge, chargeMax, range, attackDamage, attackSpeed, armor, magicResist) {
        }

        public static IEnumerable<CharacterTypeRanged[]> Values {
            get {
                yield return Archer;
            }
        }
    }
}
