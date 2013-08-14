// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
namespace Tychaia.RuntimeGeneration.Spells.Types
{
    public class ChainType : SpellType
    {
        public override double Rarity
        {
            get { return 0.25; }
        }

        public override string ToString()
        {
            return "Chain";
        }
    }
}
