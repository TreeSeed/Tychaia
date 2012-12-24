using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Spells.Modifiers
{
    [Rarity(0.015)]
    public class BounceModifier : SpellModifier
    {
        public override string ToString()
        {
            return "Bouncing";
        }
    }
}
