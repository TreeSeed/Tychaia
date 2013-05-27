using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.ProceduralGeneration.Towns
{
    public class SmallTown : Town
    {
        public SmallTown ()
        {
            this.MinSoilFertility = 0.50;       // Out of 100
            this.MinOreDensity = 0.05;           // Out of 100
            this.MinRareOreDensity = 0.0;       // Out of 100
            this.TownSize = 0.3;
            this.BrushColor = LayerColor.Red;
        }
    }
}
