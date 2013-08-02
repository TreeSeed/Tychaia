using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Elements
{
    [Rarity(0.5)]
    public class DarkElement : Element
    {
        public override string ElementTerm
        {
            get
            {
                return "Darkness";
            }
        }
        
        public override string ToString()
        {
            return "Dark";
        }
    }
}
