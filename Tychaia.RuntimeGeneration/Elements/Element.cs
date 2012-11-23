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
        virtual public string[] PresentTense
        {
            get { return new string[] { "--- Error: " + this.ToString() + " PresentTense not set ---" }; }
        }
        // Used for item generation - gives resistance to this element.
        virtual public string[] ItemPrefix
        {
            get { return new string[] { "--- Error: " + this.ToString() + " PastTense not set ---" }; }
        }
        // Used for instant damage spells.
        virtual public string[] ElementName
        {
            get { return new string[] { "--- Error: " + this.ToString() + " ElementName not set ---" }; }
        }

        public virtual string GetPresentTense()
        {
            int rand = r.Next(PresentTense.Length);
            return PresentTense[rand];
        }

        public virtual string GetItemPrefix()
        {
            int rand = r.Next(ItemPrefix.Length);
            return ItemPrefix[rand];
        }

        public virtual string GetElementName()
        {
            int rand = r.Next(ElementName.Length);
            return ElementName[rand];
        }
    }
}
