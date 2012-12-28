using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.ProceduralGeneration.Biomes
{
    public class FlatDesertSecondaryBiome : SecondaryBiome
    {
        public FlatDesertSecondaryBiome()
        {
            this.MinRainfall = 0;
            this.MaxRainfall = 0.4;
            this.MinTemperature = 0.6;
            this.MaxTemperature = 1;
            this.MinTerrain = 0;
            this.MaxTerrain = 0.4;
            this.BrushColor = LayerColor.Yellow;
            this.DefaultFor = BiomeEngine.BIOME_DESERT;
            this.TreeChance = 0.001;
            this.SuitableBiomes = new int[]
            {
                BiomeEngine.BIOME_DESERT
            };
        }
    }
}
