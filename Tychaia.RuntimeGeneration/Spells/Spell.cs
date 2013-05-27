using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tychaia.RuntimeGeneration.Elements;

namespace Tychaia.RuntimeGeneration.Spells
{
    public class Spell
    {
        private static Random r = new Random();

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

            double rand = r.NextDouble();
            if (rand >= 0.5)
            {
                return (mod + this.Element.GetWeaponPrefix() + " " + this.Type).Replace("  ", " ");
            }
            else
            {
                return (mod + this.Element.GetElementName() + " " + this.Type).Replace("  ", " ");

            }
        }
    }
}
