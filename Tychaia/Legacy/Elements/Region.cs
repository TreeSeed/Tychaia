// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tychaia.Generators;

// Currently this class is not fully implemented, so disable the
// warning about things not being used.  Remove this #pragma
// when the class is implemented and used.
#pragma warning disable 0414

namespace Tychaia.RuntimeGeneration.Elements
{
    public class Region
    {
        public const int REGION_SIZE = 16 * 10000;

        /// <summary>
        /// The chunks contained within this region.  References either the chunk or
        /// null depending on whether the chunk has been loaded into memory.
        /// </summary>
        private RuntimeChunk[,] m_Chunks = new RuntimeChunk[REGION_SIZE / 16, REGION_SIZE / 16];

        /// <summary>
        /// The neighbouring regions.
        /// </summary>
        private Region[] m_Neighbours = new Region[Direction.EIGHT_WAY];

        /// <summary>
        /// The region's name.  Randomly generated when the region is first created.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

    }
}
