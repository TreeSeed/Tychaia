using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Elements
{
    [Rarity(0.5)]
    public class LifeElement : Element
    {
        public override string[] WeaponPrefix
        {
            get
            {
                return new string[] { "Healing", "Restoring" };
            }
        }

        public override string[] ItemPrefixDamaged
        {
            get
            {
                return new string[] { "Divine", "Holy" };
            }
        }

        public override string[] ElementName
        {
            get
            {
                return new string[]
                {
                    "Heal",
                    "Life",
                    "Holy",
                    "Divine",
                    "Light"
                };
            }
        }

        public override string ToString()
        {
            return "Life";
        }
    }
}
