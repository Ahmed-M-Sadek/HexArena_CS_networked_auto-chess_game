using ASU2019_NetworkedGameWorkshop.model.grid;

namespace ASU2019_NetworkedGameWorkshop.model.character.types {
    public abstract class CharacterType {
        public const float HEIGHT = Tile.HEIGHT / 2f,
            WIDTH = HEIGHT,
            HEIGHT_HALF = HEIGHT / 2f,
            WIDTH_HALF = HEIGHT_HALF;
        public const int MAX_CHAR_LVL = 3;

        public readonly int BaseHP;
        public readonly int ChargeStarting;
        public readonly int ChargeMax;
        public readonly int Range;
        public readonly int AttackDamage;
        public readonly int AttackSpeed;
        public readonly float Armor;
        public readonly float MagicResist;

        protected CharacterType(int baseHP, int chargeStarting, int chargeMax,
            int range, int attackDamage, float attackSpeed, float armor, float magicResist) {
            BaseHP = baseHP;
            ChargeStarting = chargeStarting;
            ChargeMax = chargeMax;
            Range = range;
            AttackDamage = attackDamage;
            AttackSpeed = (int)(1000 / attackSpeed);
            Armor = armor;
            MagicResist = magicResist;
        }
    }
}
