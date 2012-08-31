﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Spells.Types
{
    public class PersonalType : SpellType
    {
        public const double Weight = 1;

        public override string ToString()
        {
            return "Personal";
        }
    }
}
