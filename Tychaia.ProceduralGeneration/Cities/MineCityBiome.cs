using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.ProceduralGeneration.CityBiomes
{
    public class Mine : CityBiome
    {
        public Mine()
        {
            this.BrushColor = LayerColor.Brown;
            this.MinOreDensity = 0.55;
        }
    }
}
