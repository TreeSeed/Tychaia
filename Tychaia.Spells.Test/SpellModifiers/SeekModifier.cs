using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Spells.Modifiers
{
    public class SeekModifier : SpellModifier
    {
        public const double Weight = 0.005;

        public override string ToString()
        {
            // TODO: Make it "Wandering" if the effect is negative.
            return "Seeking";
        }
    }
}
