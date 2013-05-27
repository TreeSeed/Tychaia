using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Elements
{
    public abstract class Element
    {
        private static Random r = new Random();
        // Need to remove changes to this so that there is just one or two for each element, removing item names that don't work as well as others.


        // Used for damage over time effects on spells, elemental weapons and shields.
        virtual public string[] WeaponPrefix
        {
            get { return new string[] { "--- Error: " + this.ToString() + " WeaponPrefix not set ---" }; }
        }

        // Used for item generation - gives resistance to this element.
        virtual public string[] ItemPrefixDamaged
        {
            get { return new string[] { "--- Error: " + this.ToString() + " ItemPrefixDamaged not set ---" }; }
        }

        // Used for instant damage spells.
        virtual public string[] ElementName
        {
            get { return new string[] { "--- Error: " + this.ToString() + " ElementName not set ---" }; }
        }

        public virtual string GetWeaponPrefix()
        {
            int rand = r.Next(WeaponPrefix.Length);
            return WeaponPrefix [rand];
        }

        public virtual string GetItemPrefixResist()
        {
            int rand = r.Next(ItemPrefixDamaged.Length);
            return ItemPrefixDamaged [rand];
        }

        public virtual string GetElementName()
        {
            int rand = r.Next(ElementName.Length);
            return ElementName [rand];
        }
    }
}
