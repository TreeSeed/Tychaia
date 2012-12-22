using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Spells.Types
{
    public class ShieldType : SpellType
    {
        public override double Rarity = 1;

        public override string ToString()
        {
            return "Shield";
        }
    }
}
