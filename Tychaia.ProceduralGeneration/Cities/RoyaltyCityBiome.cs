using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Tychaia.ProceduralGeneration.CityBiomes
{
    public class Royalty : CityBiome
    {
        public Royalty()
        {
            this.BrushColor = Color.Gold;
            this.MinMilitaryStrength = 0.25;
            this.MinSoilFertility = 0.25;
            this.MinRareOreDensity = 0.25;
        }
    }
}
