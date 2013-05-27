using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Spells.Modifiers
{
    [Rarity(0.025)]
    public class DelayModifier : SpellModifier
    {
        public override string ToString()
        {
            return "Delayed";
        }
    }
}
