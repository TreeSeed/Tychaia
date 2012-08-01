using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Tychaia.ProceduralGeneration.Biomes
{
    public class TundraSecondaryBiome : SecondaryBiome
    {
        public TundraSecondaryBiome()
        {
            this.MinRainfall = 0;
            this.MaxRainfall = 0.4;
            this.MinTemperature = 0.1;
            this.MaxTemperature = 0.4;
            this.MinTerrain = 0;
            this.MaxTerrain = 0.4;
            this.BrushColor = Color.FloralWhite;
            this.DefaultFor = BiomeEngine.BIOME_SNOW;
            this.SuitableBiomes = new int[]
            {
                BiomeEngine.BIOME_SNOW
            };
        }
    }
}
