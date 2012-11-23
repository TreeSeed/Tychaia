using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Spells.Modifiers
{
    public class BounceModifier : SpellModifier
    {
        public const double Weight = 0.015;

        public override string ToString()
        {
            return "Bouncing";
        }
    }
}
