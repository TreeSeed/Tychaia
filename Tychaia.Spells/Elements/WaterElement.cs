﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Elements
{
    public class WaterElement : Element
    {
        public const double Weight = 1;

        public override string[] PresentTense
        {
            get
            {
                return new string[] { "Drenching", "Freezing", "Chilling" };
            }
        }

        public override string[] ItemPrefix
        {
            get
            {
                return new string[] { "Drenched", "Frozen", "Chilled", "Damp", "Wet" };
            }
        }

        public override string[] ElementName
        {
            get
            {
                return new string[] { "Water", "Ice", "Liquid", "Aqua", "Chill" };
            }
        }

        public override string ToString()
        {
            return "Water";
        }
    }
}
