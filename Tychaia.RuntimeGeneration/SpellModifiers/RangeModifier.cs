using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Spells.Modifiers
{
    [Rarity(0.03)]
    public class RangeModifier : SpellModifier
    {
        public override string ToString()
        {
            // TODO: Make it "Dampened" if the effect is negative.
            return "Ranged";
        }
    }
}
