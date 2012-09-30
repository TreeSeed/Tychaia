using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Spells.Elements
{
    public class DeathElement : Element
    {
        public const double Weight = 0.5;

        public override string[] PresentTense
        {
            get
            {
                return new string[] { "Decaying", "Cursing", "Plaguing", "Ruining" };
            }
        }

        public override string[] ItemPrefix
        {
            get
            {
                return new string[] { "Darkened", "Deadly", "Plagued", "Cursed", "Decayed", "Ruined" };
            }
        }

        public override string[] ElementName
        {
            get
            {
                return new string[] { "Death", "Darkness", "Plague", "Oblivion", "Extinction", "Ruin" };
            }
        }

        public override string ToString()
        {
            return "Death";
        }
    }
}
