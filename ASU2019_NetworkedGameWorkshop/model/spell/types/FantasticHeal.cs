using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASU2019_NetworkedGameWorkshop.model.spell.types
{
    public class FantasticHeal : Spell
    {
        public FantasticHeal() :base(200,new Target(true,PriorityType.Random),SpellType.Buff) {

        }
    }
}
