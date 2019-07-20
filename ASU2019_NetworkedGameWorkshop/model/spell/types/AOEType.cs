using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASU2019_NetworkedGameWorkshop.model.spell.types
{
    public class AOEType
    {
        public static readonly AOEType Line = new AOEType("Line");
        public static readonly AOEType Circle = new AOEType("Circle");

        public static IEnumerable<AOEType> Values
        {
            get
            {
                yield return Line;
                yield return Circle;

            }
        }
        public string Name { get; private set; }
        AOEType(string name)
        {
            (Name) = (name);
        }
    }
}
