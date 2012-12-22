using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Elements
{
    public class FireElement : Element
    {
        public override double Rarity = 1;

        public override string[] PresentTense
        {
            get {
                return new string[] { "Burning", "Flaming", "Searing" };
            }
        }

        public override string[] ItemPrefix 
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
