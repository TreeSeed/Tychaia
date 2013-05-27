using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Spells.Modifiers
{
    [Rarity(0.015)]
    public class RestoreModifier : SpellModifier
    {
        public override string ToString()
        {
            // TODO: Make it "Draining" if the effect is negative.
            return "Restorative";
        }
    }
}
