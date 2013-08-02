using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Elements
{
    [Rarity(1)]
    public class IceElement : Element
    {
        public override string ElementTerm
        {
            get
            {
                return "Ice";
            }
        }

        public override string ToString()
        {
            return "Ice";
        }
    }
}
