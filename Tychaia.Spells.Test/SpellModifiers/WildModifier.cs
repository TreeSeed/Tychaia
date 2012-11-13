using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Spells.Modifiers
{
    public class WildModifier : SpellModifier
    {
        public const double Weight = 0.001;

        public override string ToString()
        {
            return "Wild";
        }
    }
}
