using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Elements
{
    public class LifeElement : Element
    {
        public override double Rarity = 0.5;

        public override string[] PresentTense
        {
            get
            {
                return new string[] { "Healing", "Restoring" };
            }
        }

        public override string[] ItemPrefix
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
                return new string[] { "Heal", "Life", "Holy", "Divine", "Light" };
            }
        }

        public override string ToString()
        {
            return "Life";
        }
    }
}
