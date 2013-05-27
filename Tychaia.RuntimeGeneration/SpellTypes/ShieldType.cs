using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Spells.Types
{
    [Rarity(1)]
    public class ShieldType : SpellType
    {
        public override string ToString()
        {
            return "Shield";
        }
    }
}
