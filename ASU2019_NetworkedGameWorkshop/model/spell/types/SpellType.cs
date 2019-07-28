using ASU2019_NetworkedGameWorkshop.model.character;
using System.Collections.Generic;

namespace ASU2019_NetworkedGameWorkshop.model.spell.types
{
    public class SpellType
    {
        public static readonly SpellType Buff = new SpellType("Buff");
        public static readonly SpellType Heal = new SpellType("Heal");
        public static readonly SpellType Damage = new SpellType("Damage");

        public static IEnumerable<SpellType> Values
        {
            get
            {
                yield return Buff;
                yield return Heal;
                yield return Damage;
            }
        }

        private StatusEffect status;

        public string Name { get; }

        public SpellType(string name) => Name = name;
        public SpellType(StatusEffect status) => this.status = status;

        public void cast(List<Character> recievers, int abilityValue)
        {
            foreach (var castee in recievers)
            {
                apply(castee, abilityValue);
            }
        }

        private void apply(Character castee, int abilityValue)
        {
            if (this == Damage)
            {
                castee.takeDamage(abilityValue, DamageType.MagicDamage);
            }
            else if (this == Heal)
            {
                castee.healHealthPoints(abilityValue);//temp
            }
            else
            {
                castee.addStatusEffect(status);
            }
        }
    }
}
