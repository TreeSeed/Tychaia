using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Spells.Types
{
    // Replaces Burst
    public class NovaType : SpellType
    {
        public override double Rarity = 0.5;

        public override string ToString()
        {
            return "Nova";
        }
    }
}
