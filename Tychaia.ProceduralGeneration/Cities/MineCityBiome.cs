using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Tychaia.ProceduralGeneration.CityBiomes
{
    public class Mine : CityBiome
    {
        public Mine()
        {
            this.BrushColor = Color.Brown;
            this.MinOreDensity = 0.25;
        }
    }
}
