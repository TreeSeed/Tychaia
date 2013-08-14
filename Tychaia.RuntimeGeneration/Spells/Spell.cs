// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Tychaia.RuntimeGeneration.Elements;

namespace Tychaia.RuntimeGeneration.Spells
{
    public class Spell
    {
        internal Spell(Element element, SpellType type, SpellModifier modifier)
        {
            this.Element = element;
            this.Type = type;
            this.Modifier = modifier;
        }

        public Element Element { get; private set; }

        public SpellType Type { get; private set; }

        public SpellModifier Modifier { get; private set; }

        public override string ToString()
        {
            var mod = "";
            if (this.Modifier.ToString() != "")
            {
                mod = this.Modifier + " ";
            }

            return (mod + this.Element + " " + this.Type).Replace("  ", " ");
        }
    }
}
