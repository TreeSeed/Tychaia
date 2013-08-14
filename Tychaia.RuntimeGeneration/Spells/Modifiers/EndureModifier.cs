// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
namespace Tychaia.RuntimeGeneration.Spells.Modifiers
{
    [Rarity(0.025)]
    public class EndureModifier : SpellModifier
    {
        public override string ToString()
        {
            // TODO: Make it "Dispersing" if the effect is negative.
            return "Enduring";
        }
    }
}
