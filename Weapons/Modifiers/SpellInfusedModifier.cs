using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Weapons.Modifiers
{
    // Attack damage after spell cast
    public class SpellInfusedModifier : Modifier
    {
        public const double Weight = 0.25;

        public override string ToString()
        {
            return "Spell Infused";
        }
    }
}
