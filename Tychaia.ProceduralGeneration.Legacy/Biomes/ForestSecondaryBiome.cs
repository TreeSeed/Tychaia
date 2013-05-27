using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.ProceduralGeneration.Biomes
{
    public class ForestSecondaryBiome : SecondaryBiome
    {
        public ForestSecondaryBiome()
        {
            this.MinRainfall = 0;
            this.MaxRainfall = 0.4;
            this.MinTemperature = 0.1;
            this.MaxTemperature = 0.4;
            this.MinTerrain = 0;
            this.MaxTerrain = 0.8;
            this.BrushColor = LayerColor.DarkGreen;
            this.DefaultFor = BiomeEngine.BIOME_FOREST;
            this.TreeChance = 0.1;
            this.SuitableBiomes = new int[]
            {
                BiomeEngine.BIOME_FOREST
            };
        }
    }
}
