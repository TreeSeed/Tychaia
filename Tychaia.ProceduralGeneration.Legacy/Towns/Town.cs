using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.ProceduralGeneration.Towns
{
    public abstract class Town
    {
        public double MinSoilFertility;     // Out of 1.00
        public double MinOreDensity;        // Out of 1.00
        public double MinRareOreDensity;    // Out of 1.00
        public double TownSize;             // How big the town will generate  // Out of 100 (lower = bigger)
        public double TownSpread;           // How many clusters of buildings the town will generate (not an actual value, just a representation %
                                            // The bigger the town is the closer the clusters will be together
        public LayerColor BrushColor;
    }
}
