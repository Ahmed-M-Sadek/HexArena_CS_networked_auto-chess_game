using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASU2019_NetworkedGameWorkshop.model.spell.types
{
    public class PriorityType
    {
        public static readonly PriorityType Close = new PriorityType("Close");
        public static readonly PriorityType Current = new PriorityType("Current");
        public static readonly PriorityType Far = new PriorityType("Far");
        public static readonly PriorityType LowHealth = new PriorityType("LowHealth");
        public static readonly PriorityType Random = new PriorityType("Random");

        public static IEnumerable<PriorityType> Values
        {
            get
            {
                yield return Close;
                yield return Current;
                yield return Far;
                yield return LowHealth;
                yield return Random;

            }
        }
        public string Name { get; private set; }
        PriorityType(string name)
        {
            (Name) = (name);
        }
    }
}
