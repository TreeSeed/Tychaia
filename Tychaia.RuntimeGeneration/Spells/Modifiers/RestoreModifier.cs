// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
namespace Tychaia.RuntimeGeneration.Spells.Modifiers
{
    [Rarity(0.015)]
    public class RestoreModifier : SpellModifier
    {
        public override string ToString()
        {
            // TODO: Make it "Draining" if the effect is negative.
            return "Restorative";
        }
    }
}