using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Spells.Types
{
    public class RayType : SpellType
    {
        public const double Weight = 0.75;

        public override string ToString()
        {
            return "Ray of";
        }
    }
}
