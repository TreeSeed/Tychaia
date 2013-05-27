using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Spells.Types
{
    [Rarity(0.3)]
    public class WallType : SpellType
    {
        public override string ToString()
        {
            return "Wall";
        }
    }
}
