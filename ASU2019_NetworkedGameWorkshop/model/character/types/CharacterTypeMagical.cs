using System.Collections.Generic;

namespace ASU2019_NetworkedGameWorkshop.model.character.types
{
    public class CharacterTypeMagical : CharacterType
    {
        public static readonly CharacterTypeMagical[] Healer = new CharacterTypeMagical[]{
            new CharacterTypeMagical(
                500,
                50, 100,
                3,
                60, 0.7f,
                25, 20),
            new CharacterTypeMagical(
                500,
                50, 100,
                3,
                60, 0.7f,
                25, 20),
            new CharacterTypeMagical(
                500,
                50, 100,
                3,
                60, 0.7f,
                25, 20),
        };

        public CharacterTypeMagical(
            int healthPoints,
            int charge, int chargeMax,
            int range,
            int attackDamage, float attackSpeed,
            int armor, int magicResist)
            : base(healthPoints, charge, chargeMax, range, attackDamage, attackSpeed, armor, magicResist)
        { }

        public new static IEnumerable<CharacterTypeMagical[]> Values
        {
            get
            {
                yield return Healer;
            }
        }
    }
}
