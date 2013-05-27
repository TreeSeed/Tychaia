using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Spells
{
    public abstract class SpellType
    {
        public virtual double Rarity { get { return 1; } }
    }
}
