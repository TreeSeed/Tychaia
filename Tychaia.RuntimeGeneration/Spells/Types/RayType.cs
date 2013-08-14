// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;

namespace Tychaia.RuntimeGeneration.Spells.Types
{
    [Rarity(0.75)]
    public class RayType : SpellType
    {
        public override string ToString()
        {
            var r = new Random();
            var rand = r.NextDouble();
            if (rand >= 0.5)
            {
                return "Ray";
            }
            return "Beam";
        }
    }
}
