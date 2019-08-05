using System.Collections.Generic;

namespace ASU2019_NetworkedGameWorkshop.model.character.types
{
    public class CharacterTypeMagical : CharacterType
    {
        public static readonly CharacterTypeMagical[] Healer = new CharacterTypeMagical[]{
            new CharacterTypeMagical(
                "Healer",
                500,
                50, 100,
                3,
                60, 0.7f,
                25, 20),
            new CharacterTypeMagical(
                "Healer",
                500,
                50, 100,
                3,
                60, 0.7f,
                25, 20),
            new CharacterTypeMagical(
                "Healer",
                500,
                50, 100,
                3,
                60, 0.7f,
                25, 20),
        };

        static CharacterTypeMagical()
        {
            foreach (var charType in Values)
            {
                foreach (var charTypeLevel in charType)
                {
                    charTypeLevel.Id = ID;
                }
                charactersList[ID++] = charType;
            }
        }
        private CharacterTypeMagical(
            string name,
            int healthPoints,
            int charge, int chargeMax,
            int range,
            int attackDamage, float attackSpeed,
            int armor, int magicResist)
            : base(name, healthPoints, charge, chargeMax, range, attackDamage, attackSpeed, armor, magicResist) { }

        /// <summary>
        /// All of the static instances of CharacterTypeMagical
        /// </summary>
        public new static IEnumerable<CharacterTypeMagical[]> Values
        {
            get
            {
                yield return Healer;
            }
        }
    }
}
