using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Spells.Types
{
    public class RingType : SpellType
    {
        // Like fire wall just around you.
        public const double Weight = 0.05;

        public override string ToString()
        {
            return "Ring";
        }
    }
}
