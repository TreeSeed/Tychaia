using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Spells.Modifiers
{
    public class EfficientModifier : SpellModifier
    {
        public override double Rarity = 0.02;

        public override string ToString()
        {
            // TODO: Make it "Inefficient" if the effect is negative.
            return "Efficient";
        }
    }
}
