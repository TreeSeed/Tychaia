using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Spells.Modifiers
{
    public class EfficientModifier : SpellModifier
    {
        public const double Weight = 0.3;

        public override string ToString()
        {
            // TODO: Make it "Inefficient" if the effect is negative.
            return "Efficient";
        }
    }
}
