using ASU2019_NetworkedGameWorkshop.model.grid;
using System.Collections.Generic;

namespace ASU2019_NetworkedGameWorkshop.model.character.types
{
    public abstract class CharacterType
    {
        public const float HEIGHT = Tile.HEIGHT / 2f,
            WIDTH = HEIGHT,
            HEIGHT_HALF = HEIGHT / 2f,
            WIDTH_HALF = HEIGHT_HALF;
        public const int MAX_CHAR_LVL = 3;

        private readonly Dictionary<StatusType, int> stats;

        public CharacterType(int healthPointsMax,
            int charge, int chargeMax,
            int range,
            int attackDamage, float attackSpeed,
            int armor, int magicResist)
            : this(healthPointsMax, healthPointsMax,
                 charge, chargeMax,
                 range,
                 attackDamage, attackSpeed,
                 armor, magicResist)
        { }

        public CharacterType(int healthPoints, int healthPointsMax,
            int charge, int chargeMax,
            int range,
            int attackDamage, float attackSpeed,
            int armor, int magicResist)
        {
            stats = new Dictionary<StatusType, int>();

            stats[StatusType.Armor] = armor;
            stats[StatusType.AttackDamage] = attackDamage;
            stats[StatusType.AttackSpeed] = (int)(1000 / attackSpeed);
            stats[StatusType.Charge] = charge;
            stats[StatusType.ChargeMax] = chargeMax;
            stats[StatusType.HealthPoints] = healthPoints;
            stats[StatusType.HealthPointsMax] = healthPointsMax;
            stats[StatusType.MagicResist] = magicResist;
            stats[StatusType.Range] = range;
        }

        public int this[StatusType index]
        {
            get { return stats[index]; }
        }


        public Dictionary<StatusType, int> statsCopy()
        {
            return new Dictionary<StatusType, int>(stats);
        }

        public static IEnumerable<IEnumerable<CharacterType[]>> Values
        {
            get
            {
                yield return CharacterTypePhysical.Values;
                yield return CharacterTypeMagical.Values;
            }
        }
    }
}
