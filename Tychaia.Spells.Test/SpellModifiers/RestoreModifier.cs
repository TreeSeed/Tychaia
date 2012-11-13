using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Spells.Modifiers
{
    public class RestoreModifier : SpellModifier
    {
        public const double Weight = 0.015;

        public override string ToString()
        {
            // TODO: Make it "Draining" if the effect is negative.
            return "Restorative";
        }
    }
}
