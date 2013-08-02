using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Elements
{
    [Rarity(1)]
    public class FireElement : Element
    {
        public override string ElementTerm
        {
            get
            {
                return "Fire";
            }
        }

        public override string ToString()
        {
            return "Fire";
        }
    }
}
