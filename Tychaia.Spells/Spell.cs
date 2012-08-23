using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Spells
{
    public class Spell
    {
        internal Spell(SpellElement element, SpellType type, SpellModifier modifier)
        {
            this.Element = element;
            this.Type = type;
            this.Modifier = modifier;
        }

        public SpellElement Element
        {
            get;
            private set;
        }

        public SpellType Type
        {
            get;
            private set;
        }

        public SpellModifier Modifier
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return (this.Type + " " + this.Modifier + " " + this.Element).Replace("  ", " ");
        }
    }
}
