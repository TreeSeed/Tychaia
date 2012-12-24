using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Weapons.Modifiers
{
    // Attack damage after spell cast
    [Rarity(0.25)]
    public class SpellInfusedModifier : WeaponModifier
    {
        public override string ToString()
        {
            return "Spell Infused";
        }
    }
}
