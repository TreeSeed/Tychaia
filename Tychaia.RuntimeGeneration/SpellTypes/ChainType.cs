using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Spells.Types
{
    public class ChainType : SpellType
    {
        public const double Weight = 0.25;

        public override string ToString()
        {
            return "Chain";
        }
    }
}
