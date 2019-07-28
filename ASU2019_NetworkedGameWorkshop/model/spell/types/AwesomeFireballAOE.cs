using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASU2019_NetworkedGameWorkshop.model.spell.types
{
    public class AwesomeFireballAOE : Spell
    {
        public AwesomeFireballAOE() :base(200,new Target(false,true,2,PriorityType.Current),SpellType.Damage) {
            name = "Spell 2";
            lbl_spell.Text = name;
        }
    }
}
