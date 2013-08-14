// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Tychaia.RuntimeGeneration.Elements;

namespace Tychaia.RuntimeGeneration.Weapons
{
    public class Weapon
    {
        private static Random r = new Random();

        internal Weapon(Element element, WeaponType type, WeaponModifier modifier)
        {
            this.Element = element;
            this.Type = type;
            this.Modifier = modifier;
        }

        public Element Element { get; private set; }

        public WeaponType Type { get; private set; }

        public WeaponModifier Modifier { get; private set; }

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
