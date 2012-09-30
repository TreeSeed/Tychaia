using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Spells
{
    public class Spell
    {
        internal Spell(Element element, SpellType type, SpellModifier modifier)
        {
            this.Element = element;
            this.Type = type;
            this.Modifier = modifier;
        }

        public Element Element
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
            string mod = "";
            if (this.Modifier.ToString() != "")
            {
                mod = this.Modifier.ToString() + " ";
            }

            return (mod + this.Element.GetPresentTense() + " " + this.Type).Replace("  ", " ");
        }
    }
}
