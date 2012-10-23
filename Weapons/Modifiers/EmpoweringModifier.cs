using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Weapons.Modifiers
{
    // Spell power per hit (goes away over time, stays for X seconds)
    // AKA: 5 sp per hit for 10 seconds, if you hit 10 times in 5 seconds then cast spells you'll have 5 seconds with 10 stacks, then lose them gradually over the next 5 seconds.
    public class EmpoweringModifier : Modifier
    {
        public const double Weight = 0.25;

        public override string ToString()
        {
            return "Empowering";
        }
    }
}
