using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Spells.Modifiers
{
    public class SpeedModifier : SpellModifier
    {
        public const double Weight = 0.04;

        public override string ToString()
        {
            // TODO: Make it "Slow" if the effect is negative.
            return "Fast";
        }
    }
}
