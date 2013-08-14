// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;

namespace Tychaia.RuntimeGeneration
{
    public class RarityAttribute : Attribute
    {
        /// <summary>
        /// Defines the rarity of an element, modifier or spell base class.
        /// </summary>
        public RarityAttribute(double rarity)
        {
            this.Rarity = rarity;
        }

        /// <summary>
        /// Gets the rarity.
        /// </summary>
        /// <value>
        /// The rarity.
        /// </value>
        public double Rarity { get; private set; }
    }
}
