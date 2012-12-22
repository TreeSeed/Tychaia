using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Weapons.Modifiers
{
    // Reduced Cooldowns
    public class QuickeningModifier : WeaponModifier
    {
        public override double Rarity = 0.25;

        public override string ToString()
        {
            return "Quickening";
        }
    }
}
