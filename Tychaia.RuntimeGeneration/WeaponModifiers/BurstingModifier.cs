using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Weapons.Modifiers
{
    [Rarity(0.25)]
    public class BurstingModifier : WeaponModifier
    {
        public override string ToString()
        {
            return "Bursting";
        }
    }
}
