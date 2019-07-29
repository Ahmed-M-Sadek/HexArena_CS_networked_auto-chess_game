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
        public string Name { get; }
        public int Id { get; set; }
        public static int ID = 0;

        private readonly static List<CharacterType[]> charactersList = new List<CharacterType[]>();
        public static CharacterType[] getCharacterType(int id)
        {
            return charactersList[id];
        }


        public CharacterType(string name,
            int healthPointsMax,
            int charge, int chargeMax,
            int range,
            int attackDamage, float attackSpeed,
            int armor, int magicResist)
            : this(name,
                  healthPointsMax, healthPointsMax,
                 charge, chargeMax,
                 range,
                 attackDamage, attackSpeed,
                 armor, magicResist)
        { }

        public CharacterType(string name,
            int healthPoints, int healthPointsMax,
            int charge, int chargeMax,
            int range,
            int attackDamage, float attackSpeed,
            int armor, int magicResist)
        {
            Name = name;
            stats = new Dictionary<StatusType, int>
            {
                [StatusType.Armor] = armor,
                [StatusType.AttackDamage] = attackDamage,
                [StatusType.AttackSpeed] = (int)(1000 / attackSpeed),
                [StatusType.Charge] = charge,
                [StatusType.ChargeMax] = chargeMax,
                [StatusType.HealthPoints] = healthPoints,
                [StatusType.HealthPointsMax] = healthPointsMax,
                [StatusType.MagicResist] = magicResist,
                [StatusType.Range] = range
            };
            charactersList.Add(this);
            this.Id = ID;
            ID++;
        }

        public int this[StatusType index]
        {
            get { return stats[index]; }
        }


        public Dictionary<StatusType, int> statsCopy()
        {
            return new Dictionary<StatusType, int>(stats);
        }

        /// <summary>
        /// the Values (IEnumerable) of the Classes that inherit from CharacterType.
        /// </summary>
        public static IEnumerable<IEnumerable<CharacterType[]>> Values
        {
            get
            {
                yield return CharacterTypePhysical.Values;
                yield return CharacterTypeMagical.Values;
            }
        }

        /// <summary>
        /// String representation of the instance
        /// </summary>
        /// <returns>Character's name</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
