using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Elements
{
    [Rarity(1)]
    public class EarthElement : Element
    {
        public override string ElementTerm
        {
            get
            {
                return "Earth";
            }
        }

        public override string ToString()
        {
            return "Earth";
        }
    }
}
