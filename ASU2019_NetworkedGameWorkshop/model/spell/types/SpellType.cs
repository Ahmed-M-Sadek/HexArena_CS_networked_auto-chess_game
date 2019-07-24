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
        public static readonly SpellType Buff = new SpellType("Buff");
        public static readonly SpellType Damage = new SpellType("Damage");




        public static IEnumerable<SpellType> Values
        {
            get
            {
                yield return Buff;
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
        public void cast(List<Character> recievers,float abilityValue)
        {
            foreach (var castee in recievers)
            {
                apply(castee, abilityValue);
            }
        }
        private void apply(Character castee,float abilityValue)
        {
            if(this == SpellType.Damage)
            {
                castee.takeDamage(abilityValue,DamageType.MagicDamage);   
            }
            else  {
                castee.addStatusEffect(Status);
            }
        }
    }
}
