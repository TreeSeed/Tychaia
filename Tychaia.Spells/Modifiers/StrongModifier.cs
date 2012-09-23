using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Spells.Modifiers
{
    public class StrongModifier : SpellModifier
    {
        public const double Weight = 0.13;

        public override string ToString()
        {
            // TODO: Make it "Weak" if the effect is negative.
            return "Strong";
        }
    }
}
