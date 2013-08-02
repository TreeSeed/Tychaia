using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Elements
{
    [Rarity(1)]
    public class WaterElement : Element
    {
        public override string ElementTerm
        {
            get
            {
                return "Water";
            }
        }

        public override string ToString()
        {
            return "Water";
        }
    }
}
