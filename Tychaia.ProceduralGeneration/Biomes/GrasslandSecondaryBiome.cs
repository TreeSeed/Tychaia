using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Tychaia.ProceduralGeneration.Biomes
{
    public class GrasslandSecondaryBiome : SecondaryBiome
    {
        public GrasslandSecondaryBiome()
        {
            this.MinRainfall = 0.2;
            this.MaxRainfall = 0.8;
            this.MinTemperature = 0.4;
            this.MaxTemperature = 0.7;
            this.MinTerrain = 0;
            this.MaxTerrain = 0.4;
            this.BrushColor = Color.Green;
            this.DefaultFor = BiomeEngine.BIOME_PLAINS;
            this.TownSuitability = 0.5;
            this.SuitableBiomes = new int[]
            {
                BiomeEngine.BIOME_PLAINS
            };
        }
    }
}
