using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Elements
{
    public class LightningElement : Element
    {
        public override double Rarity = 1;

        public override string[] PresentTense
        {
            get
            {
                return new string[] { "Shocking", "Sparking", "Jolting", "Thundering" };
            }
        }

        public override string[] ItemPrefix
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
