using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Tychaia.ProceduralGeneration.CityBiomes
{
    public class Housing : SecondaryCityBiome
    {
        public Housing()
        {
            this.BrushColor = Color.Red;
            this.RequiredOtherBiomes = 1;
        }
    }
}
