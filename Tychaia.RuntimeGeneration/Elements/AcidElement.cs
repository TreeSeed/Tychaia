using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Elements
{
    [Rarity(1)]
    public class AcidElement : Element
    {
        public override string ElementTerm
        {
            get
            {
                return "Acid";
            }
        }

        public override string ToString()
        {
            return "Acid";
        }
    }
}
