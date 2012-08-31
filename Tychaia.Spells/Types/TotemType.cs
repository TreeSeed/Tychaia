using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Spells.Types
{
    public class TotemType : SpellType
    {
        public const double Weight = 0.05;

        public override string ToString()
        {
            return "Totem of";
        }
    }
}
