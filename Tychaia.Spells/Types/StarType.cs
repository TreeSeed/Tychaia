using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Spells.Types
{
    public class StarType : SpellType
    {
        // Makes a star at target location that sends out 5 bolts after a short delay.
        public const double Weight = 1;

        public override string ToString()
        {
            return "Star";
        }
    }
}
