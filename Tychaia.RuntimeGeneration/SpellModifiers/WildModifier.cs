﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Spells.Modifiers
{
    [Rarity(0.001)]
    public class WildModifier : SpellModifier
    {
        public override string ToString()
        {
            return "Wild";
        }
    }
}
