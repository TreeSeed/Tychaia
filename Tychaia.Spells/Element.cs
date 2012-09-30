using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Spells
{
    public abstract class Element
    {
        // Used for damage over time effects on spells, elemental weapons and shields.
        virtual public string[] PresentTense
        {
            get { return new string[] { "--- Error: " + this.ToString() + " PresentTense not set ---" }; }
        }
        // Used for item generation - gives resistance to this element.
        virtual public string[] PastTense
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
            Random r = new Random();
            int rand = r.Next(PresentTense.Length);
            return PresentTense[rand];
        }

        public virtual string GetPastTense()
        {
            Random r = new Random();
            int rand = r.Next(PastTense.Length);
            return PastTense[rand];
        }

        public virtual string GetElementName()
        {
            Random r = new Random();
            int rand = r.Next(ElementName.Length);
            return ElementName[rand];
        }
        // Add for things like "get enchantment" and "get instant damage"
        // Add list for each element
    }
}
