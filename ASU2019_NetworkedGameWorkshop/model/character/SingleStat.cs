using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASU2019_NetworkedGameWorkshop.model.character {
    public class SingleStat {
        String statName { get; }
        int statValue { get; }

        public SingleStat(String name,int value) {
            statName = name;
            statValue = value;
        }

        public String Show() {
            return statName + ": " + statValue + "%";
        }
    }
}
