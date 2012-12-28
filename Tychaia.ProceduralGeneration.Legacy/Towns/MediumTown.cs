using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.ProceduralGeneration.Towns
{
    public class MediumTown : Town
    {
        public MediumTown()
        {
            this.MinSoilFertility = 0.25;       // Out of 100
            this.MinOreDensity = 0.0;           // Out of 100
            this.MinRareOreDensity = 0.0;       // Out of 100
            this.TownSize = 0.4;                // Out of 100 (lower = bigger)
            this.BrushColor = LayerColor.Blue;
        }
    }
}
