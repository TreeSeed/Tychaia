using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Spells.Modifiers
{
    public class NoneModifier : SpellModifier
    {
        public const double Weight = 0.25;

        public override string ToString()
        {
            return "";
        }
    }
}
