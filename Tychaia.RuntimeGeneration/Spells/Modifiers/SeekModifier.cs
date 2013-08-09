// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
namespace Tychaia.RuntimeGeneration.Spells.Modifiers
{
    [Rarity(0.005)]
    public class SeekModifier : SpellModifier
    {
        public override string ToString()
        {
            // TODO: Make it "Wandering" if the effect is negative.
            return "Seeking";
        }
    }
}