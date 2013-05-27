using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Spells.Modifiers
{
    [Rarity(0.04)]
    public class SpeedModifier : SpellModifier
    {
        public override string ToString()
        {
            // TODO: Make it "Slow" if the effect is negative.
            return "Fast";
        }
    }
}
