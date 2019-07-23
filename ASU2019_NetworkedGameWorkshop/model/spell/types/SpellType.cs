using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASU2019_NetworkedGameWorkshop.model.character;
using ASU2019_NetworkedGameWorkshop.controller;

namespace ASU2019_NetworkedGameWorkshop.model.spell.types
{
    public class SpellType
    {
        public static readonly SpellType Heal = new SpellType("Heal");
        public static readonly SpellType Damage = new SpellType("Damage");




        public static IEnumerable<SpellType> Values
        {
            get
            {
                yield return Heal;
                yield return Damage;

            }
        }

        public string Name { get; private set; }
        public StatusEffect Status { get; set; }
        public int Duration { get; private set; }
        public SpellType(string name)
        {
            (Name) = (name);
        }
        public SpellType(StatusEffect status)
        {
            (Status) = (status);
        }
        public void cast(List<Character> recievers,int abilityValue)
        {
            foreach (var castee in recievers)
            {
                apply(castee, abilityValue);
            }
        }
        private void apply(Character castee,int abilityValue)
        {
            if(this == SpellType.Damage)
            {
                castee.takeDamage(abilityValue,DamageType.MagicDamage);   
            }
            if(this == SpellType.Heal)
            {
                castee.healHealthPoints(abilityValue);
            }
            else  {
                castee.addStatusEffect(Status);
            }
        }
    }
}
