using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Spells.Modifiers
{
    public class RangeModifier : SpellModifier
    {
        public const double Weight = 0.03;

        public override string ToString()
        {
            // TODO: Make it "Dampened" if the effect is negative.
            return "Ranged";
        }
    }
}
