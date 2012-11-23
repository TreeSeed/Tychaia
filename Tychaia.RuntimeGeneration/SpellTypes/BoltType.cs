using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Spells.Types
{
    public class BoltType : SpellType
    {
        public const double Weight = 1;

        public override string ToString()
        {
            return "Bolt";
        }
    }
}
