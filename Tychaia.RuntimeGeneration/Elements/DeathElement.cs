using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.RuntimeGeneration.Elements
{
    [Rarity(0.5)]
    public class DeathElement : Element
    {
        public override string[] WeaponPrefix
        {
            get
            {
                return new string[]
                {
                    "Decaying",
                    "Cursing",
                    "Plaguing",
                    "Ruining"
                };
            }
        }

        public override string[] ItemPrefixDamaged
        {
            get
            {
                return new string[]
                {
                    "Darkened",
                    "Deadly",
                    "Plagued",
                    "Cursed",
                    "Decayed",
                    "Ruined"
                };
            }
        }

        public override string[] ElementName
        {
            get
            {
                return new string[]
                {
                    "Death",
                    "Darkness",
                    "Plague",
                    "Oblivion",
                    "Extinction",
                    "Ruin"
                };
            }
        }

        public override string ToString()
        {
            return "Death";
        }
    }
}
