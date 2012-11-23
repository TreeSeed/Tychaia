using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Spells.Types
{
    public class TrailType : SpellType
    {
        // Leaves a trail on the ground as you walk, damages each second.
        public const double Weight = 0.025;

        public override string ToString()
        {
            return "Trail";
        }
    }
}
