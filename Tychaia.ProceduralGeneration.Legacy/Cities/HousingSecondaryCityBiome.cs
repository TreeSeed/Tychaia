using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.ProceduralGeneration.CityBiomes
{
    public class Housing : SecondaryCityBiome
    {
        public Housing()
        {
            this.BrushColor = LayerColor.Red;
            this.RequiredOtherBiomes = 1;
        }
    }
}
