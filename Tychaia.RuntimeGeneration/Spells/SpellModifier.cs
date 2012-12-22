using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Spells
{
    public abstract class SpellModifier
    {
        // These are used to define how much 1 flat/percent is versus other modifiers.
        public abstract double Rarity = 1;
        public abstract double Flatscaling = 1;
        public abstract double Percentagescaling = 1;
        public abstract bool Canbenegative = false;
    }
}
