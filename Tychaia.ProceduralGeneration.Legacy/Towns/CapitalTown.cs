using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.ProceduralGeneration.Towns
{
    public class CapitalTown : Town
    {
        public CapitalTown()
        {
            this.MinSoilFertility = 0.60;       // Out of 100
            this.MinOreDensity = 0.2;           // Out of 100
            this.MinRareOreDensity = 0.2;       // Out of 100
            this.TownSize = 0.05;
            this.BrushColor = LayerColor.Green;
        }
    }
}
