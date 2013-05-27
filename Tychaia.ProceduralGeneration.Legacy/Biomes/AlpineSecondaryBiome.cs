using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.ProceduralGeneration.Biomes
{
    public class AlpineSecondaryBiome : SecondaryBiome
    {
        public AlpineSecondaryBiome()
        {
            this.MinRainfall = 0;
            this.MaxRainfall = 1;
            this.MinTemperature = 0;
            this.MaxTemperature = 0.4;
            this.MinTerrain = 0.7;
            this.MaxTerrain = 1;
            this.BrushColor = LayerColor.Beige;
            this.TreeChance = 0.01;
            this.SuitableBiomes = new int[]
            {
                BiomeEngine.BIOME_SNOW
            };
        }
    }
}
