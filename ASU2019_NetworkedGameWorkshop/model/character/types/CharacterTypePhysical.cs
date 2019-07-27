using System.Collections.Generic;

namespace ASU2019_NetworkedGameWorkshop.model.character.types
{
    public class CharacterTypePhysical : CharacterType
    {
        public static readonly CharacterTypePhysical[] Archer = new CharacterTypePhysical[]{
            new CharacterTypePhysical(
                "Archer",
                500,
                50, 100,
                3,
                60, 0.7f,
                25, 20),
            new CharacterTypePhysical(
                "Archer",
                500,
                50, 100,
                3,
                60, 0.7f,
                25, 20),
            new CharacterTypePhysical(
                "Archer",
                500,
                50, 100,
                3,
                60, 0.7f,
                25, 20),
        };

        public static readonly CharacterTypePhysical[] Melee = new CharacterTypePhysical[]{
            new CharacterTypePhysical(
                "Melee",
                600,
                50, 100,
                1,
                60, 0.7f,
                25, 20),
            new CharacterTypePhysical(
                "Melee",
                600,
                50, 100,
                1,
                60, 0.7f,
                25, 20),
            new CharacterTypePhysical(
                "Melee",
                600,
                50, 100,
                1,
                60, 0.7f,
                25, 20),
        };

        public CharacterTypePhysical(
            string name,
            int healthPoints,
            int charge, int chargeMax,
            int range,
            int attackDamage, float attackSpeed,
            int armor, int magicResist)
            : base(name, healthPoints, charge, chargeMax, range, attackDamage, attackSpeed, armor, magicResist)
        { }

        public new static IEnumerable<CharacterTypePhysical[]> Values
        {
            get
            {
                yield return Archer;
                yield return Melee;
            }
        }
    }
}
