using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Spells.Modifiers
{
    [Rarity(0.02)]
    public class EfficientModifier : SpellModifier
    {
        public override string ToString()
        {
            // TODO: Make it "Inefficient" if the effect is negative.
            return "Efficient";
        }
    }
}
