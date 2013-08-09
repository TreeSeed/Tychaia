// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
namespace Tychaia.RuntimeGeneration.Spells.Modifiers
{
    [Rarity(0.04)]
    public class SpeedModifier : SpellModifier
    {
        public override string ToString()
        {
            // TODO: Make it "Slow" if the effect is negative.
            return "Fast";
        }
    }
}