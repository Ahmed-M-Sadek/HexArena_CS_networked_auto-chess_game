using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASU2019_NetworkedGameWorkshop.model.spell.types
{
    public class AwesomeFireball : Spell
    {
        public AwesomeFireball() :base(200,new Target(false,PriorityType.Current),SpellType.Damage) {

        }
    }
}
