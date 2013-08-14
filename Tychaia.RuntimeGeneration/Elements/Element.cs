// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
namespace Tychaia.RuntimeGeneration.Elements
{
    public abstract class Element
    {
        // A single word descriptor, this is used for weapon enhancements, item enhancements, spells, etc
        // This is most often just the element name.
        public virtual string ElementTerm
        {
            get { return "--- Error: " + this + " ElementTerm not set ---"; }
        }

        public virtual string Description
        {
            get { return "--- Error: " + this + " Description not set ---"; }
        }
    }
}
