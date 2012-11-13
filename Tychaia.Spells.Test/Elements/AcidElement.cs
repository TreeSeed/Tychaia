using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Elements
{
    public class AcidElement : Element
    {
        public const double Weight = 1;

        public override string[] PresentTense
        {
            get
            {
                return new string[] { "Corroding", "Deteriorating", "Acidic", "Venomous" };
            }
        }

        public override string[] ItemPrefix
        {
            get
            {
                return new string[] { "Corroded", "Eroded", "Poisoned", "Deteriorated" };
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
