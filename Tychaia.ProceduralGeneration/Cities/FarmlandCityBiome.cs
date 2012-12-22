using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Tychaia.ProceduralGeneration.CityBiomes
{
    public class Farmland : CityBiome
    {
        public Farmland()
        {
            this.BrushColor = LayerColor.Green;
            this.MinSoilFertility = 0.75;
        }
    }
}
