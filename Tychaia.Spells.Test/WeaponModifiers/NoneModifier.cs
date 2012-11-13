using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Weapons.Modifiers
{
    public class NoneModifier : WeaponModifier
    {
        public const double Weight = 0.25;

        public override string ToString()
        {
            return "";
        }
    }
}
