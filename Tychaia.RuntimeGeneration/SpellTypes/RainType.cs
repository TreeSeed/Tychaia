using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Spells.Types
{
    [Rarity(0.1)]
    public class RainType : SpellType
    {
        public override string ToString()
        {
            return "Rain";
        }
    }
}
