using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Weapons.Modifiers
{
    // Spell power one
    [Rarity(0.25)]
    public class EmpoweredModifier : WeaponModifier
    {
        public override string ToString()
        {
            return "Empowered";
        }
    }
}
