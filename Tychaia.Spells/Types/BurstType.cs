using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Spells.Types
{
    public class BurstType : SpellType
    {
        public const double Weight = 0.5;

        public override string ToString()
        {
            return "Burst of";
        }
    }
}
