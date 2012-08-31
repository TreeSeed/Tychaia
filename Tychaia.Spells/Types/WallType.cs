using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Spells.Types
{
    public class WallType : SpellType
    {
        public const double Weight = 0.3;

        public override string ToString()
        {
            return "Wall of";
        }
    }
}
