using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASU2019_NetworkedGameWorkshop.model.character;
using ASU2019_NetworkedGameWorkshop.model.spell.types;

namespace ASU2019_NetworkedGameWorkshop.model.spell
{
    public class Spell
    {
        
        public int AbilityValue { get; private set; }
        public Target Target { get; private set; }
        public SpellType spellType { get; private set; }

        public Spell(int abilityValue,Target target,SpellType spellType)
        {

            AbilityValue = abilityValue;
            this.Target = target;
            this.spellType = spellType;
        }

        private List<Character> specifyTargets(Character caster)
        {
            this.Target.caster = caster;
            return Target.getTargets();
        }
        public void castSpell(Character caster)
        {
            spellType.cast(specifyTargets(caster),AbilityValue);
        }
    }
}
