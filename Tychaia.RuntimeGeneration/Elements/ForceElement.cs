using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Elements
{
    [Rarity(0.5)]
    public class ForceElement : Element
    {
        public override string ElementTerm
        {
            get
            {
                return "Force";
            }
        }

        public override string ToString()
        {
            return "Force";
        }
    }
}
