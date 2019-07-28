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
        public static readonly AwesomeFireball spell1 = new AwesomeFireball();
        public static readonly AwesomeFireballAOE spell2 = new AwesomeFireballAOE();
        public static readonly AwesomeFireballRandom spell3 = new AwesomeFireballRandom();
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
            lbl_spell.MouseClick += Skill_clicked;
        }

        private void Skill_clicked(object sender, MouseEventArgs e) {
            Shop.Shop.selectedSpell = this;
            Shop.Shop.viewSkillShop();
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
                yield return spell1;
                yield return spell2;
                yield return spell3;
            }
        }


    }
}
