// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;

namespace Tychaia.Game
{
    public class WeightedItem : Item
    {
        public Weight Weight { get; set; }

        public override float GetNumericWeight()
        {
            switch (this.Weight)
            {
                case Weight.Heavy:
                    return 5;
                case Weight.Medium:
                    return 3;
                case Weight.Light:
                    return 1;
            }
            
            throw new InvalidOperationException();
        }
    }
}
