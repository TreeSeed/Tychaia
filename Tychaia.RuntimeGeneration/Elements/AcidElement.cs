using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Elements
{
    [Rarity(1)]
    public class AcidElement : Element
    {
        public override string[] WeaponPrefix
        {
            get
            {
                return new string[] { "Corroding", "Acidic", "Venomous" };
            }
        }

        public override string[] ItemPrefixDamaged
        {
            get
            {
                return new string[]
                {
                    "Corroded",
                    "Eroded",
                    "Poisoned",
                    "Deteriorated"
                };
            }
        }

        public override string[] ElementName
        {
            get
            {
                return new string[] { "Acid", "Corrosive", "Caustic", "Toxic"};
            }
        }

        public override string ToString()
        {
            return "Acid";
        }
    }
}
