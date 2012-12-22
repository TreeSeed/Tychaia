using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Spells.Types
{
    public class SprayType : SpellType
    {
        public override double Rarity = 0.8;

        public override string ToString()
        {
            return "Spray";
        }
    }
}
