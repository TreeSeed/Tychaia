using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Elements
{
    public class FireElement : Element
    {
        public const double Weight = 1;

        public override string[] WeaponPrefix
        {
            get {
                return new string[] { "Burning", "Flaming", "Searing" };
            }
        }

        public override string[] ItemPrefixDamaged 
        {
            get
            {
                return new string[] { "Burnt", "Charred", "Seared" };
            }
        }

        public override string[] ElementName
        {
            get
            {
                return new string[] { "Fire", "Flame", "Inferno", "Heat" };
            }
        }

        public override string ToString()
        {
            return "Fire";
        }
    }
}
