using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Elements
{
    public class ForceElement : Element
    {
        public override double Rarity = 0.5;

        public override string[] WeaponPrefix
        {
            get
            {
                return new string[] { "Battering", "Bleeding", "Shredding", "Hemmorhaging" };
            }
        }

        public override string[] ItemPrefixDamaged
        {
            get
            {
                return new string[] { "Protective", "Padded", "Shielded", "Defended", "Defensive", "Protected" };
            }
        }

        public override string[] ElementName
        {
            get
            {
                return new string[] { "Force", "Physical", "Steel", "Arcane" };
            }
        }

        public override string ToString()
        {
            return "Force";
        }
    }
}
