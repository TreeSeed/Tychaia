using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Spells.Modifiers
{
    [Rarity(0.025)]
    public class EndureModifier : SpellModifier
    {
        public override string ToString()
        {
            // TODO: Make it "Dispersing" if the effect is negative.
            return "Enduring";
        }
    }
}
