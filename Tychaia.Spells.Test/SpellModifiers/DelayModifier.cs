using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Spells.Modifiers
{
    public class DelayModifier : SpellModifier
    {
        public const double Weight = 0.025;

        public override string ToString()
        {
            return "Delayed";
        }
    }
}
