// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
namespace Tychaia.RuntimeGeneration.Spells
{
    public abstract class SpellModifier
    {
        // These are used to define how much 1 flat/percent is versus other modifiers.
        public virtual double FlatScaling
        {
            get { return 1; }
        }

        public virtual double PercentageScaling
        {
            get { return 1; }
        }

        public virtual bool CanBeNegative
        {
            get { return false; }
        }
    }
}