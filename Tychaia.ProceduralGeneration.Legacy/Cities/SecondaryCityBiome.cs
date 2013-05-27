using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.ProceduralGeneration.CityBiomes
{
    public abstract class SecondaryCityBiome : CityBiome
    {
        // Requirements for biome placement
        public int RequiredOtherBiomes = 0;              // Biomes that require other biomes - Houses, Taverns, Shops, etc
    }
}
