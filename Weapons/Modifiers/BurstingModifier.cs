using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Weapons.Modifiers
{
    public class BurstingModifier : Modifier
    {
        public const double Weight = 0.25;

        public override string ToString()
        {
            return "Bursting";
        }
    }
}
