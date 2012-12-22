using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Spells.Types
{
    public class ChainType : SpellType
    {
        public override double Rarity = 0.25;

        public override string ToString()
        {
            return "Chain";
        }
    }
}
