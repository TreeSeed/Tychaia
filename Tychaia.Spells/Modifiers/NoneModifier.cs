using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Spells.Modifiers
{
    public class NoneModifier : SpellModifier
    {
        public const double Weight = 1;

        public override string ToString()
        {
            return "";
        }
    }
}
