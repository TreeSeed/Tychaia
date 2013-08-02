using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Elements
{
    [Rarity(1)]
    public class LightningElement : Element
    {
        public override string ElementTerm
        {
            get
            {
                return "Lightning";
            }
        }

        public override string ToString()
        {
            return "Lightning";
        }
    }
}
