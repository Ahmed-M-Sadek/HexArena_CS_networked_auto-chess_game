using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASU2019_NetworkedGameWorkshop.model.spell.types
{
    public class AwesomeFireballRandom : Spell
    {
        public AwesomeFireballRandom() :base(200,new Target(false,PriorityType.Random),SpellType.Damage) {
        }
    }
}
