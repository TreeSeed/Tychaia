using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Spells.Modifiers
{
    public class NoneModifier : SpellModifier
    {
        public override double Rarity = 0.25;

        public override string ToString()
        {
            return "";
        }
    }
}
