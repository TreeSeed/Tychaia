using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Elements
{
    [Rarity(1)]
    public class WaterElement : Element
    {
        public override string[] WeaponPrefix
        {
            get
            {
                return new string[] { "Drenching", "Freezing", "Chilling" };
            }
        }

        public override string[] ItemPrefixDamaged
        {
            get
            {
                return new string[]
                {
                    "Drenched",
                    "Frozen",
                    "Chilled",
                    "Damp",
                    "Wet"
                };
            }
        }

        public override string[] ElementName
        {
            get
            {
                return new string[]
                {
                    "Water",
                    "Ice",
                    "Liquid",
                    "Aqua",
                    "Chill"
                };
            }
        }

        public override string ToString()
        {
            return "Water";
        }
    }
}
