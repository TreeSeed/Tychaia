// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
namespace Tychaia.RuntimeGeneration.Elements
{
    [Rarity(1)]
    public class AcidElement : Element
    {
        public override string ElementTerm
        {
            get { return "Acid"; }
        }

        public override string ToString()
        {
            return "Acid";
        }
    }
}
