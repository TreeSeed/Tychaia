using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Spells.Types
{
    [Rarity(0.75)]
    public class RayType : SpellType
    {
        public override string ToString()
        {
            Random r = new Random();
            double rand = r.NextDouble();
            if (rand >= 0.5)
            {
                return "Ray";
            } else
            {
                return "Beam";
            }
        }
    }
}
