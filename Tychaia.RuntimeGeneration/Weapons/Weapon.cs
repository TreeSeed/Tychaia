using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tychaia.RuntimeGeneration.Elements;

namespace Tychaia.RuntimeGeneration.Weapons
{
    public class Weapon
    {
        private static Random r = new Random();

        internal Weapon(Element element, WeaponType type, WeaponModifier modifier)
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

        public WeaponType Type
        {
            get;
            private set;
        }

        public WeaponModifier Modifier
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
                return (mod + this.Element.GetItemPrefix() + " " + this.Type).Replace("  ", " ");
            }
            else
            {
                return (mod + this.Element.GetElementName() + " " + this.Type).Replace("  ", " ");

            }
        }
    }
}
