using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.ProceduralGeneration.Biomes
{
    public class OceanBiome : SecondaryBiome
    {
        public OceanBiome()
        {
            this.MinRainfall = 0;
            this.MaxRainfall = 1;
            this.MinTemperature = 0;
            this.MaxTemperature = 1;
            this.MinTerrain = 0;
            this.MaxTerrain = 0;
            this.BrushColor = LayerColor.Blue;
            this.DefaultFor = BiomeEngine.BIOME_OCEAN;
            this.TreeChance = 0;
            this.SuitableBiomes = new int[]
            {
                BiomeEngine.BIOME_OCEAN
            };
        }
    }
}
