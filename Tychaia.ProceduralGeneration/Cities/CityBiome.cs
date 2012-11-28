using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Tychaia.ProceduralGeneration.CityBiomes
{
    public abstract class CityBiome
    {
        // Requirements for biome placement
        public double MinSoilFertility = 0;           // Out of 1.00 (Used for placing food generation and sometimes water generation)
        public double MaxSoilFertility = 1;
        public double MinOreDensity = 0;              // Out of 1.00 (Used for placing mines)
        public double MaxOreDensity = 1;
        public double MinRareOreDensity = 0;          // Out of 1.00 (Used for placing prestiege style buildings, such as castles and festival grounds)
        public double MaxRareOreDensity = 1;
        public double MinAnimalDensity = 0;
        public double MaxAnimalDensity = 1.0;
        public double MinMilitaryStrength = 0;        // Out of 1.00 (Used for placing military area such as barrcks and training grounds)
        public double MaxMilitaryStrength = 1;
        public double MinWaterValue = 0.0;            // Required water value from the secondary biome for this building to be placed.
        public double MaxWaterValue = 1.0;            // Out of 1.00
        public double MinHeatValue = 0.0;             // Required heat value from the secondary biome for this building to be placed. 
        public double MaxHeatValue = 1.0;             // Out of 1.00
        public double MinHeight = 0;
        public double MaxHeight = 1.0;

        // Biome Statistics
        public double CitySpread = 1;                // Spread of this biome - average cells buildings are apart
        public double CitizenWealth = 1;             // Arbitrary value defining how wealthy the poplulation of this biome is.

        // Color that this biome draws
        public Color BrushColor;
    }
}
