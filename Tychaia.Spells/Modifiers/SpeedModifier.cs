using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Spells.Modifiers
{
    public class SpeedModifier : SpellModifier
    {
        public override string ToString()
        {
            // TODO: Make it "Slow" if the effect is negative.
            return "Fast";
        }
    }
}
