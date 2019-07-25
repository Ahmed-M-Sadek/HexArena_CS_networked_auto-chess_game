using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ASU2019_NetworkedGameWorkshop.model.character;
using ASU2019_NetworkedGameWorkshop.model.spell.types;

namespace ASU2019_NetworkedGameWorkshop.model.spell
{
    public class Spell {
        public int AbilityValue { get; private set; }
        public Target Target { get; private set; }
        public SpellType spellType { get; private set; }
        public String name { get; protected set; }
        public Label lbl_spell { get; protected set; }

        public Spell(int abilityValue,Target target,SpellType spellType)
        {

            AbilityValue = abilityValue;
            this.Target = target;
            this.spellType = spellType;

            lbl_spell = new Label();
            lbl_spell.MouseClick += skill_clicked;
        }

        private void skill_clicked(object sender, MouseEventArgs e) {
            throw new NotImplementedException();
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
        public static IEnumerable<Spell> Values {
            get {
                yield return new AwesomeFireball();
                yield return new AwesomeFireballAOE();
                yield return new AwesomeFireballRandom();
            }
        }


    }
}
