using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Elements
{
    public class LightningElement : Element
    {
        public const double Weight = 1;

        public override string[] WeaponPrefix
        {
            get
            {
                return new string[] { "Shocking", "Sparking", "Jolting", "Thundering" };
            }
        }

        public override string[] ItemPrefixDamaged
        {
            get
            {
                return new string[] { "Charged", "Ionized", "Conducting" };
            }
        }

        public override string[] ElementName
        {
            get
            {
                return new string[] { "Lightning", "Electric", "Spark", "Shock", "Jolt" };
            }
        }

        public override string ToString()
        {
            return "Lightning";
        }
    }
}
