using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Spells.Modifiers
{
    public class MulticastModifier : SpellModifier
    {
        public const double Weight = 0.001;

        public override string ToString()
        {
            return "Multicasting";
        }
    }
}
