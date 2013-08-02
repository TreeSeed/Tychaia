using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Elements
{
    public abstract class Element
    {
        // A single word descriptor, this is used for weapon enhancements, item enhancements, spells, etc
        // This is most often just the element name.
        virtual public string ElementTerm
        {
            get { return "--- Error: " + this.ToString() + " ElementTerm not set ---"; }
        }

        virtual public string Description
        {
            get { return "--- Error: " + this.ToString() + " Description not set ---"; }
        }

    }
}
