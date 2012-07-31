using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Tychaia.ProceduralGeneration.Biomes
{
    public abstract class SecondaryBiome
    {
        public double MinRainfall;
        public double MaxRainfall;
        public double MinTemperature;
        public double MaxTemperature;
        public double MinTerrain;
        public double MaxTerrain;
        public int[] SuitableBiomes;
        public Color BrushColor;
    }
}
