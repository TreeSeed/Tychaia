using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Weapons.Modifiers
{
    // Reduced Cooldowns
    public class QuickeningModifier : Modifier
    {
        public const double Weight = 0.25;

        public override string ToString()
        {
            return "Quickening";
        }
    }
}
