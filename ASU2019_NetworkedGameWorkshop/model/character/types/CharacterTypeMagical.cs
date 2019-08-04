using ASU2019_NetworkedGameWorkshop.Properties;
using System.Collections.Generic;
using System.Drawing;

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
                25, 20,Resources.Healer1Blue,Resources.Healer1Red),
            new CharacterTypeMagical(
                "Healer",
                500,
                50, 100,
                3,
                60, 0.7f,
                25, 20,Resources.Healer2Blue,Resources.Healer2Red),
            new CharacterTypeMagical(
                "Healer",
                500,
                50, 100,
                3,
                60, 0.7f,
                25, 20,Resources.Healer3Blue,Resources.Healer3Red),
        };

        private CharacterTypeMagical(
            string name,
            int healthPoints,
            int charge, int chargeMax,
            int range,
            int attackDamage, float attackSpeed,
            int armor, int magicResist,Bitmap imageBlue,Bitmap imageRed)
            : base(name, healthPoints, charge, chargeMax, range, attackDamage, attackSpeed, armor, magicResist, imageBlue, imageRed) { }

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
