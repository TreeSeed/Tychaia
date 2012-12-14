using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.ProceduralGeneration.Biomes
{
    public abstract class SecondaryBiome
    {
        // Requirements for biome placement
        public double MinRainfall;
        public double MaxRainfall;
        public double MinTemperature;
        public double MaxTemperature;
        public double MinTerrain;
        public double MaxTerrain;
        public double TownSuitability;

        // Building assistance
        public double BuildingMaterial;       // The building material that the buildings are made out of in this biome
        public double HeatValue = 0.5;        // The amount of heat that is in this biome
        public double WaterValue = 0.5;       // The amount of water that is in this biome
        
        // Primary to Secondary biomes
        public int[] SuitableBiomes;
        public int DefaultFor = -1;

        // Color that this biome draws
        public LayerColor BrushColor;
    }
}
