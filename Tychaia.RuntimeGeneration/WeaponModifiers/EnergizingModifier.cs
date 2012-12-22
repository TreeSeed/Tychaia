using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Weapons.Modifiers
{
    public class EnergizingModifier : WeaponModifier
    {
        public override double Rarity = 0.25;

        public override string ToString()
        {
            return "Energizing";
        }
    }
}
