using System.Collections.Generic;

namespace ASU2019_NetworkedGameWorkshop.model.character {
    public class CharacterStats {
        SingleStat damage { get; }
        SingleStat defence { get; }
        SingleStat dodgeChance { get; }

        public List<SingleStat> list { get; }
        public CharacterStats() : this(50, 50, 50) {

        }
        public CharacterStats(int DMG, int DEF, int DDG) {
            list = new List<SingleStat>();

            damage = new SingleStat("DMG", DMG);
            defence = new SingleStat("DEF", DEF);
            dodgeChance = new SingleStat("DDG", DDG);

            list.Add(damage);
            list.Add(defence);
            list.Add(dodgeChance);
        }
    }
}
