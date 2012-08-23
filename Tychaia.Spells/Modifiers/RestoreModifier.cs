using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Spells.Modifiers
{
    public class RestoreModifier : SpellModifier
    {
        public override string ToString()
        {
            // TODO: Make it "Draining" if the effect is negative.
            return "Restorative";
        }
    }
}
