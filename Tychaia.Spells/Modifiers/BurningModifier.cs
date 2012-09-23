using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Spells.Modifiers
{
    public class BurningModifier : SpellModifier
    {
        public const double Weight = 0.025;

        public override string ToString()
        {
            // TODO: Make it chose an element, derive DoT name from elements.
            return "Burning";
        }
    }
}
