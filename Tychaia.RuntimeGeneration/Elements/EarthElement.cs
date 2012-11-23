using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Elements
{
    public class EarthElement : Element
    {
        public const double Weight = 1;

        public override string[] PresentTense
        {
            get
            {
                return new string[] { "Stoning", "Rumbling", "Sand Storm" };
            }
        }

        public override string[] ItemPrefix
        {
            get
            {
                return new string[] { "Rock Solid", "Stoned", "Earthen" };
            }
        }

        public override string[] ElementName
        {
            get
            {
                return new string[] { "Rock", "Stone", "Earth" };
            }
        }

        public override string ToString()
        {
            return "Earth";
        }
    }
}
