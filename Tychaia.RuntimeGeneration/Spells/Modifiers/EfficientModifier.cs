// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
namespace Tychaia.RuntimeGeneration.Spells.Modifiers
{
    [Rarity(0.02)]
    public class EfficientModifier : SpellModifier
    {
        public override string ToString()
        {
            // TODO: Make it "Inefficient" if the effect is negative.
            return "Efficient";
        }
    }
}