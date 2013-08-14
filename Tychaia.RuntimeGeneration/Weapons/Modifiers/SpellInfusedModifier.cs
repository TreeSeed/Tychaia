// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
namespace Tychaia.RuntimeGeneration.Weapons.Modifiers
{
    // Attack damage after spell cast
    [Rarity(0.25)]
    public class SpellInfusedModifier : WeaponModifier
    {
        public override string ToString()
        {
            return "Spell Infused";
        }
    }
}
