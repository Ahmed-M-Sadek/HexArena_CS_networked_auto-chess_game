using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASU2019_NetworkedGameWorkshop.model.spell.types
{
    public class AwesomeFireballAOE : Spell
    {
        public AwesomeFireballAOE() :base(200,new Target(false,true,AOEType.Circle,PriorityType.Current),SpellType.Damage) {

        }
    }
}
